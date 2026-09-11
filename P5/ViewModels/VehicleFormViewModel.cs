using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using P5.Models;
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

    [Required(ErrorMessage = "Le modèle et la finition sont obligatoires.")]
    [Range(1, int.MaxValue, ErrorMessage = "Le modèle et la finition sont obligatoires.")]
    [Display(Name = "Modèle et finition")]
    public int TrimId { get; set; }

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

    [StringLength(260)]
    [Display(Name = "Photo")]
    public string? PhotoUrl { get; set; }

    /// <summary>Alimentee par le controleur, jamais postee.</summary>
    [BindNever]
    [ValidateNever]
    public IEnumerable<SelectListItem> Trims { get; set; } = [];

    public static VehicleFormViewModel FromEntity(Vehicle v) => new()
    {
        Id = v.Id,
        Vin = v.Vin,
        Year = v.Year,
        TrimId = v.TrimId,
        PurchaseDate = v.PurchaseDate,
        PurchasePrice = v.PurchasePrice,
        AvailabilityDate = v.AvailabilityDate,
        Description = v.Description,
        PhotoUrl = v.PhotoUrl
    };

    /// <summary>Reporte les champs saisis sur l'entite, sans jamais toucher SaleDate.</summary>
    public void ApplyTo(Vehicle v)
    {
        v.Vin = Vin;
        v.Year = Year;
        v.TrimId = TrimId;
        v.PurchaseDate = PurchaseDate;
        v.PurchasePrice = PurchasePrice;
        v.AvailabilityDate = AvailabilityDate;
        v.Description = Description;
        v.PhotoUrl = PhotoUrl;
    }

    public Vehicle ToEntity()
    {
        var vehicle = new Vehicle { Id = Id };
        ApplyTo(vehicle);
        return vehicle;
    }
}
