
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using P5.Models;
using P5.Data;

namespace P5.Controllers;

public class VehiculesController : Controller
{
    private readonly ApplicationDbContext _context;

    public VehiculesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: VEHICULES
    public async Task<IActionResult> Index()
    {
        // Les Include sont indispensables : sans Reparations, CoutReparations vaut 0
        // et PrixVente affiche PrixAchat + 500 €, ce qui fausse la regle metier.
        var vehicules = await _context.Vehicules
            .Include(v => v.ModeleVoiture)
                .ThenInclude(m => m!.Marque)
            .Include(v => v.Reparations)
            .AsNoTracking()
            .ToListAsync();

        return View(vehicules);
    }

    // GET: VEHICULES/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var vehicule = await _context.Vehicules
            .Include(v => v.ModeleVoiture)
                .ThenInclude(m => m!.Marque)
            .Include(v => v.Reparations)
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);
        if (vehicule == null)
        {
            return NotFound();
        }

        return View(vehicule);
    }

    // GET: VEHICULES/Create
    public async Task<IActionResult> Create()
    {
        await PeuplerListeModelesAsync();
        return View();
    }

    // POST: VEHICULES/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,CodeVin,Annee,ModeleVoitureId,ModeleVoiture,Finition,DateAchat,PrixAchat,DateDisponibilite,DateVente,Description,PhotoUrl,Reparations,CoutReparations,PrixVente,EstDisponible")] Vehicule vehicule)
    {
        if (ModelState.IsValid)
        {
            _context.Add(vehicule);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Le formulaire est reaffiche : la liste deroulante doit etre reconstruite,
        // elle ne survit pas au POST.
        await PeuplerListeModelesAsync(vehicule.ModeleVoitureId);
        return View(vehicule);
    }

    // GET: VEHICULES/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var vehicule = await _context.Vehicules.FindAsync(id);
        if (vehicule == null)
        {
            return NotFound();
        }

        await PeuplerListeModelesAsync(vehicule.ModeleVoitureId);
        return View(vehicule);
    }

    // POST: VEHICULES/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,CodeVin,Annee,ModeleVoitureId,ModeleVoiture,Finition,DateAchat,PrixAchat,DateDisponibilite,DateVente,Description,PhotoUrl,Reparations,CoutReparations,PrixVente,EstDisponible")] Vehicule vehicule)
    {
        if (id != vehicule.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(vehicule);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VehiculeExists(vehicule.Id))
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

        await PeuplerListeModelesAsync(vehicule.ModeleVoitureId);
        return View(vehicule);
    }

    // GET: VEHICULES/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var vehicule = await _context.Vehicules
            .FirstOrDefaultAsync(m => m.Id == id);
        if (vehicule == null)
        {
            return NotFound();
        }

        return View(vehicule);
    }

    // POST: VEHICULES/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var vehicule = await _context.Vehicules.FindAsync(id);
        if (vehicule != null)
        {
            _context.Vehicules.Remove(vehicule);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool VehiculeExists(int? id)
    {
        return _context.Vehicules.Any(e => e.Id == id);
    }

    /// <summary>
    /// Alimente la liste deroulante des modeles, libelles "Marque Modele" : un nom de
    /// modele seul serait ambigu, et un identifiant numerique inutilisable.
    /// </summary>
    private async Task PeuplerListeModelesAsync(int? modeleSelectionne = null)
    {
        var modeles = await _context.ModelesVoiture
            .Include(m => m.Marque)
            .OrderBy(m => m.Marque!.Nom)
            .ThenBy(m => m.Nom)
            .Select(m => new { m.Id, Libelle = m.Marque!.Nom + " " + m.Nom })
            .AsNoTracking()
            .ToListAsync();

        ViewBag.ModeleVoitureId = new SelectList(modeles, "Id", "Libelle", modeleSelectionne);
    }
}
