using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using P5.Models;
using P5.Services;
using P5.Validation;

namespace P5.ViewModels;

/// <summary>
/// Surface de saisie des formulaires Create et Edit. SaleDate, les propriétés
/// calculées et les navigations n'y figurent pas.
/// </summary>
public class VehicleFormViewModel
{
    public int Id { get; set; }

    [StringLength(17, MinimumLength = 17, ErrorMessage = "Le code VIN comporte exactement 17 caractères.")]
    [Display(Name = "Code VIN")]
    public string? Vin { get; set; }

    // [Required] explicite, pour un message d'erreur en français sur ce type valeur.
    [Required(ErrorMessage = "L'année est obligatoire.")]
    [VehicleYear]
    [Display(Name = "Année")]
    public int Year { get; set; }

    [Required(ErrorMessage = "La marque est obligatoire.")]
    [StringLength(50, ErrorMessage = "50 caractères maximum.")]
    [Display(Name = "Marque")]
    public string BrandName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le modèle est obligatoire.")]
    [StringLength(50, ErrorMessage = "50 caractères maximum.")]
    [Display(Name = "Modèle")]
    public string ModelName { get; set; } = string.Empty;

    [Required(ErrorMessage = "La finition est obligatoire.")]
    [StringLength(30, ErrorMessage = "30 caractères maximum.")]
    [Display(Name = "Finition")]
    public string TrimName { get; set; } = string.Empty;

    /// <summary>
    /// Jamais postée : le contrôleur la déduit des trois noms saisis, en retrouvant
    /// ou en créant la finition dans le catalogue.
    /// </summary>
    [BindNever]
    [ValidateNever]
    public int TrimId { get; set; }

    [Required(ErrorMessage = "La date d'achat est obligatoire.")]
    [DataType(DataType.Date)]
    [Display(Name = "Date d'achat")]
    public DateOnly PurchaseDate { get; set; }

    [Required(ErrorMessage = "Le prix d'achat est obligatoire.")]
    [Range(0, 1_000_000, ErrorMessage = "Le prix d'achat doit être positif.")]
    [Display(Name = "Prix d'achat")]
    public decimal PurchasePrice { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Disponible à la vente le")]
    public DateOnly? AvailabilityDate { get; set; }

    [StringLength(1000)]
    [DataType(DataType.MultilineText)]
    [Display(Name = "Description de l'annonce")]
    public string? Description { get; set; }

    /// <summary>Fichier téléversé, facultatif : la photo actuelle est conservée si aucun fichier n'est envoyé.</summary>
    [Display(Name = "Photo")]
    public IFormFile? Photo { get; set; }

    /// <summary>
    /// Adresse de la photo actuelle, pour l'affichage dans le formulaire.
    /// Jamais postée : c'est le service de stockage qui la fabrique.
    /// </summary>
    [BindNever]
    [ValidateNever]
    public string? PhotoUrl { get; set; }

    /// <summary>Suggestions de saisie, alimentées par le contrôleur, jamais postées.</summary>
    [BindNever]
    [ValidateNever]
    public CatalogueNames? Catalogue { get; set; }

    /// <summary>Marge de la configuration, pour l'aperçu du prix de vente. Affichée, jamais postée.</summary>
    [BindNever]
    [ValidateNever]
    public decimal Margin { get; set; }

    /// <summary>Coût des réparations déjà saisies, pour l'aperçu du prix de vente. Affiché, jamais posté.</summary>
    [BindNever]
    [ValidateNever]
    public decimal RepairsCost { get; set; }

    public static VehicleFormViewModel FromEntity(Vehicle v) => new()
    {
        Id = v.Id,
        Vin = v.Vin,
        Year = v.Year,
        TrimId = v.TrimId,
        BrandName = v.Trim?.CarModel?.Brand?.Name ?? string.Empty,
        ModelName = v.Trim?.CarModel?.Name ?? string.Empty,
        TrimName = v.Trim?.Name ?? string.Empty,
        PurchaseDate = v.PurchaseDate,
        PurchasePrice = v.PurchasePrice,
        AvailabilityDate = v.AvailabilityDate,
        Description = v.Description,
        PhotoUrl = v.PhotoUrl
    };

    /// <summary>Reporte les champs saisis sur l'entité, sans jamais toucher SaleDate ni PhotoUrl (gérée par le contrôleur).</summary>
    public void ApplyTo(Vehicle v)
    {
        v.Vin = Vin;
        v.Year = Year;
        v.TrimId = TrimId;
        v.PurchaseDate = PurchaseDate;
        v.PurchasePrice = PurchasePrice;
        v.AvailabilityDate = AvailabilityDate;
        v.Description = Description;
    }

    public Vehicle ToEntity()
    {
        var vehicle = new Vehicle { Id = Id };
        ApplyTo(vehicle);
        return vehicle;
    }
}
