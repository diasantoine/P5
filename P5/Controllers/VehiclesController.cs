
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using P5.Models;
using P5.Data;

namespace P5.Controllers;

public class VehiclesController : Controller
{
    private readonly ApplicationDbContext _context;

    public VehiclesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: VEHICLES
    public async Task<IActionResult> Index()
    {
        // Les Include sont indispensables : sans Repairs, RepairsCost vaut 0
        // et SalePrice affiche PurchasePrice + 500 €, ce qui fausse la regle metier.
        // La marque et le modele ne sont accessibles qu'a travers la finition :
        // Vehicle -> Trim -> CarModel -> Brand, d'ou les deux ThenInclude.
        var vehicles = await _context.Vehicles
            .Include(v => v.Trim)
                .ThenInclude(t => t!.CarModel)
                .ThenInclude(m => m!.Brand)
            .Include(v => v.Repairs)
            .AsNoTracking()
            .ToListAsync();

        return View(vehicles);
    }

    // GET: VEHICLES/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var vehicle = await _context.Vehicles
            .Include(v => v.Trim)
                .ThenInclude(t => t!.CarModel)
                .ThenInclude(m => m!.Brand)
            .Include(v => v.Repairs)
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);
        if (vehicle == null)
        {
            return NotFound();
        }

        return View(vehicle);
    }

    // GET: VEHICLES/Create
    public async Task<IActionResult> Create()
    {
        await PopulateTrimListAsync();
        return View();
    }

    // POST: VEHICLES/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Vin,Year,TrimId,PurchaseDate,PurchasePrice,AvailabilityDate,SaleDate,Description,PhotoUrl,Repairs,RepairsCost,SalePrice,IsAvailable")] Vehicle vehicle)
    {
        if (ModelState.IsValid)
        {
            _context.Add(vehicle);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Le formulaire est reaffiche : la liste deroulante doit etre reconstruite,
        // elle ne survit pas au POST.
        await PopulateTrimListAsync(vehicle.TrimId);
        return View(vehicle);
    }

    // GET: VEHICLES/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var vehicle = await _context.Vehicles
            .Include(v => v.Trim)
                .ThenInclude(t => t!.CarModel)
                .ThenInclude(m => m!.Brand)
            .Include(v => v.Repairs)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (vehicle == null)
        {
            return NotFound();
        }

        await PopulateTrimListAsync(vehicle.TrimId);
        return View(vehicle);
    }

    // POST: VEHICLES/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
            try
            {
                _context.Update(vehicle);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VehicleExists(vehicle.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }

        await PopulateTrimListAsync(vehicle.TrimId);
        return View(vehicle);
    }

    // GET: VEHICLES/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var vehicle = await _context.Vehicles
            .Include(v => v.Trim)
                .ThenInclude(t => t!.CarModel)
                .ThenInclude(m => m!.Brand)
            .Include(v => v.Repairs)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (vehicle == null)
        {
            return NotFound();
        }

        return View(vehicle);
    }

    // POST: VEHICLES/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var vehicle = await _context.Vehicles.FindAsync(id);
        if (vehicle != null)
        {
            _context.Vehicles.Remove(vehicle);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool VehicleExists(int? id)
    {
        return _context.Vehicles.Any(e => e.Id == id);
    }

    /// <summary>
    /// Alimente la liste deroulante des finitions, libellees "Marque Modele Finition" :
    /// une finition seule ("LE") serait ambigue, et un identifiant numerique inutilisable.
    /// Une seule liste pour les trois niveaux du catalogue : c'est la finition qui
    /// porte la reference, la marque et le modele en decoulent.
    /// </summary>
    private async Task PopulateTrimListAsync(int? selectedTrimId = null)
    {
        var trims = await _context.Trims
            .AsNoTracking()
            .OrderBy(t => t.CarModel!.Brand!.Name)
            .ThenBy(t => t.CarModel!.Name)
            .ThenBy(t => t.Name)
            .Select(t => new { t.Id, Label = t.CarModel!.Brand!.Name + " " + t.CarModel.Name + " " + t.Name })
            .ToListAsync();

        ViewBag.TrimId = new SelectList(trims, "Id", "Label", selectedTrimId);
    }
}
