using P5.Models;

namespace P5.Services;

public interface IRepairService
{
    /// <summary>Enregistre une reparation et retourne son identifiant.</summary>
    Task<int> AddAsync(Repair repair);

    /// <summary>Supprime une reparation et retourne l'identifiant de son vehicule, ou null.</summary>
    Task<int?> DeleteAsync(int id);

    Task<bool> VehicleExistsAsync(int vehicleId);
}
