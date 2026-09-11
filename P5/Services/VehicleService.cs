using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using P5.Configuration;
using P5.Data;
using P5.Models;

namespace P5.Services;

public class VehicleService(ApplicationDbContext context, IOptions<PricingOptions> pricing) : IVehicleService
{
    private readonly ApplicationDbContext _context = context;
    private readonly decimal _margin = pricing.Value.FixedMargin;

    /// <summary>
    /// Les Include sont indispensables : sans Repairs, RepairsCost vaut 0 et SalePrice
    /// affiche PurchasePrice + 500 euros ; sans les trois ThenInclude, Designation ne
    /// peut pas lire la marque, le modele et la finition sur la specification.
    /// Les centraliser ici garantit qu'aucun appelant ne peut les oublier.
    /// </summary>
    private IQueryable<Vehicle> WithDependencies() =>
        _context.Vehicles
            .Include(v => v.Specification!).ThenInclude(s => s.Brand)
            .Include(v => v.Specification!).ThenInclude(s => s.CarModel)
            .Include(v => v.Specification!).ThenInclude(s => s.Trim)
            .Include(v => v.Repairs);

    public async Task<IReadOnlyList<Vehicle>> GetInventoryAsync()
    {
        var inventory = await WithDependencies()
            .OrderBy(v => v.SaleDate == null ? 0 : 1)
            .ThenByDescending(v => v.PurchaseDate)
            .AsNoTracking()
            .ToListAsync();

        foreach (var vehicle in inventory)
        {
            ApplyMargin(vehicle);
        }

        return inventory;
    }

    public async Task<Vehicle?> GetDetailAsync(int id)
    {
        var vehicle = await WithDependencies()
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == id);

        return vehicle is null ? null : ApplyMargin(vehicle);
    }

    /// <summary>
    /// Applique la marge lue dans la configuration. Tout véhicule destiné à l'affichage passe
    /// par ici : c'est ce qui garantit qu'aucun n'affiche la marge par défaut de l'entité.
    /// </summary>
    private Vehicle ApplyMargin(Vehicle vehicle)
    {
        vehicle.Margin = _margin;
        return vehicle;
    }

    public async Task<Vehicle?> GetForEditAsync(int id) =>
        await _context.Vehicles.FirstOrDefaultAsync(v => v.Id == id);

    public async Task<int> AddAsync(Vehicle vehicle)
    {
        _context.Vehicles.Add(vehicle);
        await _context.SaveChangesAsync();
        return vehicle.Id;
    }

    public async Task<bool> UpdateAsync(Vehicle vehicle)
    {
        if (!await _context.Vehicles.AnyAsync(v => v.Id == vehicle.Id))
        {
            return false;
        }

        _context.Vehicles.Update(vehicle);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> MarkAsSoldAsync(int id, DateOnly saleDate)
    {
        var vehicle = await _context.Vehicles.FirstOrDefaultAsync(v => v.Id == id);
        // Une vente déjà enregistrée ne s'écrase pas : il faudrait d'abord remettre le véhicule en vente.
        if (vehicle is null || vehicle.SaleDate is not null || saleDate < vehicle.PurchaseDate)
        {
            return false;
        }

        vehicle.SaleDate = saleDate;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var vehicle = await _context.Vehicles.FirstOrDefaultAsync(v => v.Id == id);
        if (vehicle is null)
        {
            return false;
        }

        _context.Vehicles.Remove(vehicle);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IReadOnlyList<SpecificationOption>> GetSpecificationOptionsAsync() =>
        await _context.VehicleSpecifications
            .OrderBy(s => s.Brand!.Name)
            .ThenBy(s => s.CarModel!.Name)
            .ThenBy(s => s.Trim!.Name)
            .Select(s => new SpecificationOption(s.Id, s.Brand!.Name + " " + s.CarModel!.Name + " " + s.Trim!.Name))
            .AsNoTracking()
            .ToListAsync();
}
