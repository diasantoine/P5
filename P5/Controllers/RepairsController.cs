using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using P5.Models;
using P5.Services;
using P5.ViewModels;

namespace P5.Controllers;

/// <summary>
/// Aucune action publique : les reparations relevent entierement du back-office.
/// Toutes les redirections ramenent sur la fiche du vehicule concerne.
/// </summary>
[Authorize]
public class RepairsController(IRepairService repairs, IVehicleService vehicles) : Controller
{
    private readonly IRepairService _repairs = repairs;
    private readonly IVehicleService _vehicles = vehicles;

    public async Task<IActionResult> Create(int vehicleId)
    {
        var vehicle = await _vehicles.GetDetailAsync(vehicleId);
        if (vehicle is null)
        {
            return NotFound();
        }

        return View(new RepairFormViewModel
        {
            VehicleId = vehicleId,
            VehicleDesignation = BuildDesignation(vehicle)
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RepairFormViewModel form)
    {
        var vehicle = await _vehicles.GetDetailAsync(form.VehicleId);
        if (vehicle is null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            // VehicleDesignation n'est jamais reposte (cf. RepairFormViewModel) : il faut
            // la reconstituer ici pour que le formulaire renvoye ne l'affiche pas vide.
            form.VehicleDesignation = BuildDesignation(vehicle);
            return View(form);
        }

        await _repairs.AddAsync(form.ToEntity());
        return RedirectToAction("Details", "Vehicles", new { id = form.VehicleId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var vehicleId = await _repairs.DeleteAsync(id);
        return vehicleId is null
            ? NotFound()
            : RedirectToAction("Details", "Vehicles", new { id = vehicleId });
    }

    private static string BuildDesignation(Vehicle vehicle) =>
        $"{vehicle.Trim?.CarModel?.Brand?.Name} {vehicle.Trim?.CarModel?.Name} {vehicle.Trim?.Name}".Trim();
}
