using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using P5.Models;

namespace P5.ViewModels;

public class RepairFormViewModel
{
    [Required]
    public int VehicleId { get; set; }

    /// <summary>Affiche à l'utilisateur de quel véhicule il s'agit. Jamais reposté.</summary>
    [BindNever]
    [ValidateNever]
    public string VehicleDesignation { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le libellé de la réparation est obligatoire.")]
    [StringLength(200, ErrorMessage = "200 caractères maximum.")]
    [Display(Name = "Réparation")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le coût est obligatoire.")]
    [Range(0, 100_000, ErrorMessage = "Le coût doit être positif.")]
    [Display(Name = "Coût")]
    public decimal Cost { get; set; }

    public Repair ToEntity() => new()
    {
        Description = Description,
        Cost = Cost,
        VehicleId = VehicleId
    };
}
