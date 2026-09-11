using Microsoft.EntityFrameworkCore;
using P5.Data;
using P5.Models;

namespace P5.Services;

public class RepairService(ApplicationDbContext context) : IRepairService
{
    private readonly ApplicationDbContext _context = context;

    public async Task<int> AddAsync(Repair repair)
    {
        _context.Repairs.Add(repair);
        await _context.SaveChangesAsync();
        return repair.Id;
    }

    public async Task<int?> DeleteAsync(int id)
    {
        var repair = await _context.Repairs.FirstOrDefaultAsync(r => r.Id == id);
        if (repair is null)
        {
            return null;
        }

        var vehicleId = repair.VehicleId;
        _context.Repairs.Remove(repair);
        await _context.SaveChangesAsync();
        return vehicleId;
    }

    public Task<bool> VehicleExistsAsync(int vehicleId) =>
        _context.Vehicles.AnyAsync(v => v.Id == vehicleId);
}
