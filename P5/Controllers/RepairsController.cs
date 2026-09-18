using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using P5.Security;
using P5.Services;
using P5.ViewModels;

namespace P5.Controllers;

/// <summary>
/// Aucune action publique : les réparations relèvent entièrement du back-office, réservé au gérant.
/// Toutes les redirections ramènent sur la fiche du véhicule concerné.
/// </summary>
[Authorize(Roles = AppRoles.Admin)]
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
            VehicleDesignation = vehicle.Designation
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RepairFormViewModel form)
    {
        if (!await _repairs.VehicleExistsAsync(form.VehicleId))
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            // VehicleDesignation n'est jamais reposté (cf. RepairFormViewModel) : elle est
            // reconstituée ici pour que le formulaire renvoyé ne l'affiche pas vide.
            var vehicle = await _vehicles.GetDetailAsync(form.VehicleId);
            form.VehicleDesignation = vehicle?.Designation ?? string.Empty;
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
}
