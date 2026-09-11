using Microsoft.EntityFrameworkCore;
using P5.Data;
using P5.Models;

namespace P5.Services;

public class VehicleService(ApplicationDbContext context) : IVehicleService
{
    private readonly ApplicationDbContext _context = context;

    /// <summary>
    /// Les deux Include sont indispensables : sans Repairs, RepairsCost vaut 0
    /// et SalePrice affiche PurchasePrice + 500 euros. La marque et le modele ne sont
    /// accessibles qu'a travers la finition (Vehicle -> Trim -> CarModel -> Brand),
    /// d'ou les deux ThenInclude en chaine. Les centraliser ici garantit qu'aucun
    /// appelant ne peut les oublier.
    /// </summary>
    private IQueryable<Vehicle> WithDependencies() =>
        _context.Vehicles
            .Include(v => v.Trim)
                .ThenInclude(t => t!.CarModel)
                .ThenInclude(m => m!.Brand)
            .Include(v => v.Repairs);

    public async Task<IReadOnlyList<Vehicle>> GetInventoryAsync() =>
        await WithDependencies()
            .OrderBy(v => v.SaleDate == null ? 0 : 1)
            .ThenByDescending(v => v.PurchaseDate)
            .AsNoTracking()
            .ToListAsync();

    public async Task<Vehicle?> GetDetailAsync(int id) =>
        await WithDependencies()
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == id);

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
        if (vehicle is null)
        {
            return false;
        }

        vehicle.SaleDate = saleDate;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IReadOnlyList<TrimOption>> GetTrimOptionsAsync() =>
        await _context.Trims
            .Include(t => t.CarModel)
                .ThenInclude(m => m!.Brand)
            .OrderBy(t => t.CarModel!.Brand!.Name)
            .ThenBy(t => t.CarModel!.Name)
            .ThenBy(t => t.Name)
            .Select(t => new TrimOption(t.Id, t.CarModel!.Brand!.Name + " " + t.CarModel.Name + " " + t.Name))
            .AsNoTracking()
            .ToListAsync();
}
