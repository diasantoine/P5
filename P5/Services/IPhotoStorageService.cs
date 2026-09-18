namespace P5.Services;

/// <summary>
/// Stockage des photos d'annonces. Le controleur ne sait pas ou ni comment les fichiers sont ecrits :
/// on pourrait passer du disque a un stockage en ligne sans le toucher.
/// </summary>
public interface IPhotoStorageService
{
    /// <summary>Message d'erreur en francais si le fichier est refuse, null s'il est acceptable.</summary>
    string? Validate(IFormFile file);

    /// <summary>Ecrit le fichier sous un nom aleatoire et retourne son adresse, a ranger dans Vehicle.PhotoUrl.</summary>
    Task<string> SaveAsync(IFormFile file);

    /// <summary>Supprime une photo televersee. Ignore toute adresse qui ne vient pas de ce service.</summary>
    void Delete(string? photoUrl);
}
