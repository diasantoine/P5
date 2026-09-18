namespace P5.Services;

/// <summary>Noms déjà présents dans le catalogue, proposés en suggestion sous les champs du formulaire.</summary>
public record CatalogueNames(
    IReadOnlyList<string> Brands,
    IReadOnlyList<string> Models,
    IReadOnlyList<string> Trims);
