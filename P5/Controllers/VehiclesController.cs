using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using P5.Configuration;
using P5.Security;
using P5.Services;
using P5.ViewModels;

namespace P5.Controllers;

// Fermé par défaut : seules Index et Details restent accessibles à tous.
// Écrire dans l'inventaire exige le rôle du gérant.
[Authorize(Roles = AppRoles.Admin)]
public class VehiclesController(IVehicleService vehicles, IPhotoStorageService photos, IOptions<PricingOptions> pricing) : Controller
{
    private readonly IVehicleService _vehicles = vehicles;
    private readonly IPhotoStorageService _photos = photos;
    private readonly PricingOptions _pricing = pricing.Value;

    [AllowAnonymous]
    public async Task<IActionResult> Index() => View(await _vehicles.GetInventoryAsync());

    [AllowAnonymous]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var vehicle = await _vehicles.GetDetailAsync(id.Value);
        if (vehicle == null)
        {
            return NotFound();
        }

        return View(vehicle);
    }

    public async Task<IActionResult> Create()
    {
        var form = new VehicleFormViewModel { Year = DateTime.Today.Year, PurchaseDate = DateOnly.FromDateTime(DateTime.Today) };
        await PopulateFormAsync(form);
        return View(form);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(VehicleFormViewModel form)
    {
        ValidatePhoto(form);
        if (!ModelState.IsValid)
        {
            await PopulateFormAsync(form);
            return View(form);
        }

        string? newPhotoUrl = null;
        try
        {
            form.SpecificationId = await _vehicles.GetOrCreateSpecificationIdAsync(form.BrandName, form.ModelName, form.TrimName);
            var vehicle = form.ToEntity();
            if (form.Photo is not null)
            {
                vehicle.PhotoUrl = newPhotoUrl = await _photos.SaveAsync(form.Photo);
            }

            var id = await _vehicles.AddAsync(vehicle);
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (DbUpdateException ex)
        {
            // L'écriture a échoué : la photo déjà enregistrée sur le disque est supprimée.
            _photos.Delete(newPhotoUrl);
            AddSaveFailureError(ex, form);
            await PopulateFormAsync(form);
            return View(form);
        }
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var vehicle = await _vehicles.GetDetailAsync(id.Value);
        if (vehicle is null)
        {
            return NotFound();
        }

        var form = VehicleFormViewModel.FromEntity(vehicle);
        await PopulateFormAsync(form);
        return View(form);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, VehicleFormViewModel form)
    {
        if (id != form.Id)
        {
            return NotFound();
        }

        ValidatePhoto(form);
        if (!ModelState.IsValid)
        {
            await PopulateFormAsync(form);
            return View(form);
        }

        // On recharge l'entité suivie : les champs absents du formulaire (SaleDate,
        // les réparations) conservent leur valeur en base.
        var vehicle = await _vehicles.GetForEditAsync(id);
        if (vehicle is null)
        {
            return NotFound();
        }

        var previousPhotoUrl = vehicle.PhotoUrl;
        string? newPhotoUrl = null;
        try
        {
            form.SpecificationId = await _vehicles.GetOrCreateSpecificationIdAsync(form.BrandName, form.ModelName, form.TrimName);
            form.ApplyTo(vehicle);
            if (form.Photo is not null)
            {
                vehicle.PhotoUrl = newPhotoUrl = await _photos.SaveAsync(form.Photo);
            }

            await _vehicles.UpdateAsync(vehicle);

            // L'ancienne photo n'est supprimée qu'une fois la nouvelle enregistrée en base.
            if (newPhotoUrl is not null)
            {
                _photos.Delete(previousPhotoUrl);
            }

            return RedirectToAction(nameof(Details), new { id });
        }
        catch (DbUpdateException ex)
        {
            _photos.Delete(newPhotoUrl);
            AddSaveFailureError(ex, form);
            await PopulateFormAsync(form);
            return View(form);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAsSold(int id, DateOnly? saleDate)
    {
        var date = saleDate ?? DateOnly.FromDateTime(DateTime.Today);

        if (date > DateOnly.FromDateTime(DateTime.Today))
        {
            return BadRequest();
        }

        if (!await _vehicles.MarkAsSoldAsync(id, date))
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var vehicle = await _vehicles.GetDetailAsync(id.Value);
        return vehicle is null ? NotFound() : View(vehicle);
    }

    // La suppression efface les réparations en cascade : elle passe par une confirmation
    // et n'est jamais atteignable par un lien.
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var photoUrl = (await _vehicles.GetDetailAsync(id))?.PhotoUrl;
        if (!await _vehicles.DeleteAsync(id))
        {
            return NotFound();
        }

        _photos.Delete(photoUrl);
        return RedirectToAction(nameof(Index));
    }

    // Tout ce que le formulaire affiche sans jamais le poster : à reconstituer à chaque renvoi de la vue.
    private async Task PopulateFormAsync(VehicleFormViewModel form)
    {
        form.Catalogue = await _vehicles.GetCatalogueNamesAsync();

        // Aperçu du prix de vente : la marge vient de la configuration, les réparations de la base.
        form.Margin = _pricing.FixedMargin;

        // En modification, le coût des réparations et la photo actuelle viennent de la base, jamais du formulaire.
        var current = form.Id == 0 ? null : await _vehicles.GetDetailAsync(form.Id);
        form.RepairsCost = current?.RepairsCost ?? 0m;
        form.PhotoUrl = current?.PhotoUrl;
    }

    private void ValidatePhoto(VehicleFormViewModel form)
    {
        if (form.Photo is not null && _photos.Validate(form.Photo) is { } error)
        {
            ModelState.AddModelError(nameof(form.Photo), error);
        }
    }

    // Traduit un échec d'écriture en erreur de formulaire : sur le champ VIN si l'index unique
    // du VIN est en cause, générale sinon.
    private void AddSaveFailureError(DbUpdateException exception, VehicleFormViewModel form)
    {
        if (IsVinUniqueViolation(exception))
        {
            ModelState.AddModelError(nameof(form.Vin), "Ce code VIN est déjà utilisé.");
        }
        else
        {
            ModelState.AddModelError(string.Empty, "L'enregistrement a échoué : vérifiez les informations saisies.");
        }
    }

    private static bool IsVinUniqueViolation(DbUpdateException exception)
    {
        for (Exception? current = exception; current is not null; current = current.InnerException)
        {
            if (current.Message.Contains("IX_Vehicles_Vin", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}
