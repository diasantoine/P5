namespace P5.Services;

/// <summary>
/// Stockage des photos d'annonces. Le contrôleur ne connaît pas l'emplacement
/// ni le mode d'écriture des fichiers.
/// </summary>
public interface IPhotoStorageService
{
    /// <summary>Message d'erreur en français si le fichier est refusé, null s'il est acceptable.</summary>
    string? Validate(IFormFile file);

    /// <summary>Écrit le fichier sous un nom aléatoire et retourne son adresse, à ranger dans Vehicle.PhotoUrl.</summary>
    Task<string> SaveAsync(IFormFile file);

    /// <summary>Supprime une photo téléversée. Ignore toute adresse qui ne vient pas de ce service.</summary>
    void Delete(string? photoUrl);
}
