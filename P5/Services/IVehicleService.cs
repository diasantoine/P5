using P5.Models;

namespace P5.Services;

/// <summary>
/// Contrat metier de l'inventaire. Le controleur ne parle qu'a cette interface :
/// il ne connait ni EF Core, ni les Include necessaires au calcul du prix de vente.
/// </summary>
public interface IVehicleService
{
    /// <summary>Inventaire complet, reparations et marques chargees.</summary>
    Task<IReadOnlyList<Vehicle>> GetInventoryAsync();

    /// <summary>Fiche complete en lecture seule, ou null si l'identifiant est inconnu.</summary>
    Task<Vehicle?> GetDetailAsync(int id);

    /// <summary>Vehicule suivi par le contexte, destine a une modification.</summary>
    Task<Vehicle?> GetForEditAsync(int id);

    /// <summary>Enregistre un nouveau vehicule et retourne son identifiant.</summary>
    Task<int> AddAsync(Vehicle vehicle);

    /// <summary>Retourne false si le vehicule n'existe plus.</summary>
    Task<bool> UpdateAsync(Vehicle vehicle);

    /// <summary>Retire le vehicule de la vente en renseignant sa date de vente.</summary>
    Task<bool> MarkAsSoldAsync(int id, DateOnly saleDate);

    /// <summary>Specifications disponibles, libellees "Marque Modele Finition", triees.</summary>
    Task<IReadOnlyList<SpecificationOption>> GetSpecificationOptionsAsync();
}
