namespace P5.Services;

public class PhotoStorageService(IWebHostEnvironment environment) : IPhotoStorageService
{
    public const long MaxSizeInBytes = 5 * 1024 * 1024;
    private const string UrlPrefix = "/uploads/vehicles/";

    // Extensions acceptées, avec les premiers octets attendus pour chacune.
    private static readonly Dictionary<string, byte[][]> Signatures = new(StringComparer.OrdinalIgnoreCase)
    {
        [".jpg"] = [[0xFF, 0xD8, 0xFF]],
        [".jpeg"] = [[0xFF, 0xD8, 0xFF]],
        [".png"] = [[0x89, 0x50, 0x4E, 0x47]],
        [".webp"] = [[0x52, 0x49, 0x46, 0x46]]
    };

    private readonly string _folder = Path.Combine(environment.WebRootPath, "uploads", "vehicles");

    public string? Validate(IFormFile file)
    {
        if (file.Length == 0)
        {
            return "Le fichier est vide.";
        }

        if (file.Length > MaxSizeInBytes)
        {
            return "La photo ne doit pas dépasser 5 Mo.";
        }

        if (!Signatures.TryGetValue(Path.GetExtension(file.FileName), out var signatures))
        {
            return "Formats acceptés : JPEG, PNG ou WebP.";
        }

        using var stream = file.OpenReadStream();
        var header = new byte[4];
        var read = stream.Read(header, 0, header.Length);
        var matches = signatures.Any(s => read >= s.Length && header.Take(s.Length).SequenceEqual(s));

        return matches ? null : "Le contenu du fichier n'est pas une image valide.";
    }

    public async Task<string> SaveAsync(IFormFile file)
    {
        Directory.CreateDirectory(_folder);

        // Le fichier est enregistré sous un nom aléatoire, pas sous celui envoyé par le navigateur.
        var fileName = $"{Guid.NewGuid():N}{Path.GetExtension(file.FileName).ToLowerInvariant()}";

        await using var target = File.Create(Path.Combine(_folder, fileName));
        await file.CopyToAsync(target);

        return UrlPrefix + fileName;
    }

    public void Delete(string? photoUrl)
    {
        // Ne supprime que les adresses produites par ce service.
        if (string.IsNullOrEmpty(photoUrl) || !photoUrl.StartsWith(UrlPrefix, StringComparison.Ordinal))
        {
            return;
        }

        // GetFileName écarte tout « ../ » : on ne supprime que dans le dossier des photos.
        var path = Path.Combine(_folder, Path.GetFileName(photoUrl));
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}
