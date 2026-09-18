using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using P5.Models;
using P5.Services;
using P5.Validation;

namespace P5.ViewModels;

/// <summary>
/// Surface de saisie des formulaires Create et Edit. Volontairement plus pauvre que
/// l'entite : SaleDate, les proprietes calculees et les navigations n'y figurent pas,
/// donc aucun champ poste ne peut les atteindre (over-posting).
/// </summary>
public class VehicleFormViewModel
{
    public int Id { get; set; }

    [StringLength(17, MinimumLength = 17, ErrorMessage = "Le code VIN comporte exactement 17 caractères.")]
    [Display(Name = "Code VIN")]
    public string? Vin { get; set; }

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
    /// Jamais postee : le controleur la deduit des trois noms saisis, en retrouvant ou en creant
    /// l'entree de catalogue. Un formulaire forge ne peut donc pas viser une specification arbitraire.
    /// </summary>
    [BindNever]
    [ValidateNever]
    public int SpecificationId { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Date d'achat")]
    public DateOnly PurchaseDate { get; set; }

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

    /// <summary>Fichier televerse. Facultatif : sans lui, l'annonce garde sa photo actuelle.</summary>
    [Display(Name = "Photo")]
    public IFormFile? Photo { get; set; }

    /// <summary>
    /// Adresse de la photo actuelle, pour l'afficher dans le formulaire. Jamais postee : c'est le
    /// service de stockage qui la fabrique, sinon un formulaire forge pourrait pointer n'importe ou.
    /// </summary>
    [BindNever]
    [ValidateNever]
    public string? PhotoUrl { get; set; }

    /// <summary>Suggestions de saisie, alimentees par le controleur, jamais postees.</summary>
    [BindNever]
    [ValidateNever]
    public CatalogueNames? Catalogue { get; set; }

    /// <summary>Marge de la configuration, pour l'apercu du prix de vente. Affichee, jamais postee.</summary>
    [BindNever]
    [ValidateNever]
    public decimal Margin { get; set; }

    /// <summary>Cout des reparations deja saisies, pour l'apercu du prix de vente. Affiche, jamais poste.</summary>
    [BindNever]
    [ValidateNever]
    public decimal RepairsCost { get; set; }

    public static VehicleFormViewModel FromEntity(Vehicle v) => new()
    {
        Id = v.Id,
        Vin = v.Vin,
        Year = v.Year,
        SpecificationId = v.SpecificationId,
        BrandName = v.Specification?.Brand?.Name ?? string.Empty,
        ModelName = v.Specification?.CarModel?.Name ?? string.Empty,
        TrimName = v.Specification?.Trim?.Name ?? string.Empty,
        PurchaseDate = v.PurchaseDate,
        PurchasePrice = v.PurchasePrice,
        AvailabilityDate = v.AvailabilityDate,
        Description = v.Description,
        PhotoUrl = v.PhotoUrl
    };

    /// <summary>Reporte les champs saisis sur l'entite, sans jamais toucher SaleDate ni PhotoUrl (geree par le controleur).</summary>
    public void ApplyTo(Vehicle v)
    {
        v.Vin = Vin;
        v.Year = Year;
        v.SpecificationId = SpecificationId;
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
