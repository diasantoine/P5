using P5.Models;

namespace P5.Services;

/// <summary>
/// Contrat métier de l'inventaire. Le contrôleur ne parle qu'à cette interface :
/// il ne connaît ni EF Core, ni les Include nécessaires au calcul du prix de vente.
/// </summary>
public interface IVehicleService
{
    /// <summary>Inventaire complet, réparations et marques chargées.</summary>
    Task<IReadOnlyList<Vehicle>> GetInventoryAsync();

    /// <summary>Fiche complète en lecture seule, ou null si l'identifiant est inconnu.</summary>
    Task<Vehicle?> GetDetailAsync(int id);

    /// <summary>Véhicule suivi par le contexte, destiné à une modification.</summary>
    Task<Vehicle?> GetForEditAsync(int id);

    /// <summary>Enregistre un nouveau véhicule et retourne son identifiant.</summary>
    Task<int> AddAsync(Vehicle vehicle);

    /// <summary>Retourne false si le véhicule n'existe plus.</summary>
    Task<bool> UpdateAsync(Vehicle vehicle);

    /// <summary>Retire le véhicule de la vente en renseignant sa date de vente.</summary>
    Task<bool> MarkAsSoldAsync(int id, DateOnly saleDate);

    /// <summary>Supprime le véhicule et, en cascade, ses réparations. False si l'identifiant est inconnu.</summary>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// Retrouve la finition sous ce modèle et cette marque, ou crée ce qui manque dans le catalogue.
    /// La comparaison ignore la casse et les espaces de bord ; la graphie déjà en base est conservée.
    /// </summary>
    Task<int> GetOrCreateTrimIdAsync(string brandName, string modelName, string trimName);

    /// <summary>Noms déjà connus du catalogue, pour suggérer la saisie sans l'imposer.</summary>
    Task<CatalogueNames> GetCatalogueNamesAsync();
}
