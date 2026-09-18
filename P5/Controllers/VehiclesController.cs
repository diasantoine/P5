using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using P5.Configuration;
using P5.Security;
using P5.Services;
using P5.ViewModels;

namespace P5.Controllers;

// Fermé par défaut ; seules Index et Details, les pages publiques de la vitrine, restent accessibles à tous.
// L'inscription est ouverte : être connecté ne suffit donc pas, il faut le rôle du gérant pour écrire.
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
            // L'annonce n'a pas ete enregistree : sa photo ne doit pas rester orpheline sur le disque.
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

        // On recharge l'entite suivie : les champs absents du formulaire (SaleDate,
        // les reparations) conservent ainsi leur valeur en base.
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

            // L'ancienne photo n'est supprimee qu'une fois la nouvelle enregistree en base.
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

    // Tout ce que le formulaire affiche sans jamais le poster : a refaire a chaque renvoi de la vue.
    private async Task PopulateFormAsync(VehicleFormViewModel form)
    {
        form.Catalogue = await _vehicles.GetCatalogueNamesAsync();

        // Apercu du prix de vente : la marge vient de la configuration, les reparations de la base.
        form.Margin = _pricing.FixedMargin;

        // En modification, le cout des reparations et la photo actuelle viennent de la base, jamais du formulaire.
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

    // L'unicite du VIN n'est verifiable qu'en base (contrainte SQL) : seul l'echec de l'ecriture la revele.
    // Un autre DbUpdateException (ex. SpecificationId inconnu) ne doit pas accuser le VIN a tort.
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
