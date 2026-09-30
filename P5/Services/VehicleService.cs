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
    /// Charge les réparations et la chaîne finition, modèle, marque, nécessaires au calcul
    /// du prix de vente et à la désignation du véhicule.
    /// </summary>
    private IQueryable<Vehicle> WithDependencies() =>
        _context.Vehicles
            .Include(v => v.Trim!).ThenInclude(t => t.CarModel!).ThenInclude(m => m.Brand)
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
    /// Applique la marge lue dans la configuration à un véhicule destiné à l'affichage.
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
        // Une vente déjà enregistrée ne s'écrase pas.
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

    /// <summary>
    /// Chaque niveau n'est recherché que si son parent existe déjà : un modèle ne peut pas
    /// exister sous une marque tout juste créée. Les entités nouvelles sont reliées par leurs
    /// navigations et enregistrées en une seule transaction.
    /// </summary>
    public async Task<int> GetOrCreateTrimIdAsync(string brandName, string modelName, string trimName)
    {
        brandName = brandName.Trim();
        modelName = modelName.Trim();
        trimName = trimName.Trim();

        var brand = await _context.Brands
            .FirstOrDefaultAsync(b => b.Name.ToLower() == brandName.ToLower())
            ?? new Brand { Name = brandName };

        var carModel = (brand.Id == 0 ? null : await _context.CarModels
            .FirstOrDefaultAsync(m => m.BrandId == brand.Id && m.Name.ToLower() == modelName.ToLower()))
            ?? new CarModel { Name = modelName, Brand = brand };

        var trim = (carModel.Id == 0 ? null : await _context.Trims
            .FirstOrDefaultAsync(t => t.CarModelId == carModel.Id && t.Name.ToLower() == trimName.ToLower()))
            ?? new Trim { Name = trimName, CarModel = carModel };

        if (trim.Id == 0)
        {
            _context.Trims.Add(trim);
            await _context.SaveChangesAsync();
        }

        return trim.Id;
    }

    public async Task<CatalogueNames> GetCatalogueNamesAsync() => new(
        await _context.Brands.Select(b => b.Name).Distinct().OrderBy(n => n).ToListAsync(),
        await _context.CarModels.Select(m => m.Name).Distinct().OrderBy(n => n).ToListAsync(),
        await _context.Trims.Select(t => t.Name).Distinct().OrderBy(n => n).ToListAsync());
}
