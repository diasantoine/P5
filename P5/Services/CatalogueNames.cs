namespace P5.Services;

/// <summary>Noms deja presents dans le catalogue, proposes en suggestion sous les champs du formulaire.</summary>
public record CatalogueNames(
    IReadOnlyList<string> Brands,
    IReadOnlyList<string> Models,
    IReadOnlyList<string> Trims);
