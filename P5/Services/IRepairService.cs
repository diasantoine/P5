using P5.Models;

namespace P5.Services;

public interface IRepairService
{
    /// <summary>Enregistre une réparation et retourne son identifiant.</summary>
    Task<int> AddAsync(Repair repair);

    /// <summary>Supprime une réparation et retourne l'identifiant de son véhicule, ou null.</summary>
    Task<int?> DeleteAsync(int id);

    Task<bool> VehicleExistsAsync(int vehicleId);
}
