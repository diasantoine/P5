using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using P5.Models;
using P5.Services;

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
        await PopulateTrimListAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Vin,Year,TrimId,PurchaseDate,PurchasePrice,AvailabilityDate,SaleDate,Description,PhotoUrl,Repairs,RepairsCost,SalePrice,IsAvailable")] Vehicle vehicle)
    {
        if (ModelState.IsValid)
        {
            await _vehicles.AddAsync(vehicle);
            return RedirectToAction(nameof(Index));
        }

        // Le formulaire est reaffiche : la liste deroulante doit etre reconstruite,
        // elle ne survit pas au POST.
        await PopulateTrimListAsync(vehicle.TrimId);
        return View(vehicle);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var vehicle = await _vehicles.GetForEditAsync(id.Value);
        if (vehicle == null)
        {
            return NotFound();
        }

        await PopulateTrimListAsync(vehicle.TrimId);
        return View(vehicle);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,Vin,Year,TrimId,PurchaseDate,PurchasePrice,AvailabilityDate,SaleDate,Description,PhotoUrl,Repairs,RepairsCost,SalePrice,IsAvailable")] Vehicle vehicle)
    {
        if (id != vehicle.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            if (!await _vehicles.UpdateAsync(vehicle))
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        await PopulateTrimListAsync(vehicle.TrimId);
        return View(vehicle);
    }

    /// <summary>
    /// Alimente la liste deroulante des finitions, libellees "Marque Modele Finition" :
    /// une finition seule ("LE") serait ambigue, et un identifiant numerique inutilisable.
    /// </summary>
    private async Task PopulateTrimListAsync(int? selectedTrimId = null)
    {
        var trims = await _vehicles.GetTrimOptionsAsync();
        ViewBag.TrimId = new SelectList(trims, nameof(TrimOption.Id), nameof(TrimOption.Label), selectedTrimId);
    }
}
