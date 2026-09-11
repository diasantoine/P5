using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using P5.Services;
using P5.ViewModels;

namespace P5.Controllers;

public class VehiclesController(IVehicleService vehicles) : Controller
{
    private readonly IVehicleService _vehicles = vehicles;

    public async Task<IActionResult> Index() => View(await _vehicles.GetInventoryAsync());

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
        await PopulateTrimListAsync(form);
        return View(form);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(VehicleFormViewModel form)
    {
        if (!ModelState.IsValid)
        {
            await PopulateTrimListAsync(form);
            return View(form);
        }

        var id = await _vehicles.AddAsync(form.ToEntity());
        return RedirectToAction(nameof(Details), new { id });
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
        await PopulateTrimListAsync(form);
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

        if (!ModelState.IsValid)
        {
            await PopulateTrimListAsync(form);
            return View(form);
        }

        // On recharge l'entite suivie : les champs absents du formulaire (SaleDate,
        // les reparations) conservent ainsi leur valeur en base.
        var vehicle = await _vehicles.GetForEditAsync(id);
        if (vehicle is null)
        {
            return NotFound();
        }

        form.ApplyTo(vehicle);
        await _vehicles.UpdateAsync(vehicle);
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAsSold(int id, DateOnly? saleDate)
    {
        var date = saleDate ?? DateOnly.FromDateTime(DateTime.Today);

        if (!await _vehicles.MarkAsSoldAsync(id, date))
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    private async Task PopulateTrimListAsync(VehicleFormViewModel form)
    {
        var options = await _vehicles.GetTrimOptionsAsync();
        form.Trims = options.Select(o => new SelectListItem(o.Label, o.Id.ToString()));
    }
}
