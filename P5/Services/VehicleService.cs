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

    /// <summary>
    /// Chaque niveau n'est cherche que si son parent existe deja : un modele ne peut pas exister
    /// sous une marque qu'on vient de creer. Les entites nouvelles sont reliees par leurs navigations,
    /// EF en deduit les cles etrangeres (y compris les composites) et tout part en un seul
    /// SaveChanges, donc en une seule transaction : le catalogue n'est jamais a moitie ecrit.
    /// </summary>
    public async Task<int> GetOrCreateSpecificationIdAsync(string brandName, string modelName, string trimName)
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

        var specification = trim.Id == 0 ? null : await _context.VehicleSpecifications
            .FirstOrDefaultAsync(s => s.BrandId == brand.Id && s.CarModelId == carModel.Id && s.TrimId == trim.Id);

        if (specification is null)
        {
            specification = new VehicleSpecification { Brand = brand, CarModel = carModel, Trim = trim };
            _context.VehicleSpecifications.Add(specification);
            await _context.SaveChangesAsync();
        }

        return specification.Id;
    }

    public async Task<CatalogueNames> GetCatalogueNamesAsync() => new(
        await _context.Brands.Select(b => b.Name).Distinct().OrderBy(n => n).ToListAsync(),
        await _context.CarModels.Select(m => m.Name).Distinct().OrderBy(n => n).ToListAsync(),
        await _context.Trims.Select(t => t.Name).Distinct().OrderBy(n => n).ToListAsync());

    public async Task<IReadOnlyList<SpecificationOption>> GetSpecificationOptionsAsync() =>
        await _context.VehicleSpecifications
            .OrderBy(s => s.Brand!.Name)
            .ThenBy(s => s.CarModel!.Name)
            .ThenBy(s => s.Trim!.Name)
            .Select(s => new SpecificationOption(s.Id, s.Brand!.Name + " " + s.CarModel!.Name + " " + s.Trim!.Name))
            .AsNoTracking()
            .ToListAsync();
}
