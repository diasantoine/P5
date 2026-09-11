using System.ComponentModel.DataAnnotations;
using P5.Models;

namespace P5.ViewModels;

public class RepairFormViewModel
{
    [Required]
    public int VehicleId { get; set; }

    /// <summary>Affiche a l'utilisateur de quel vehicule il s'agit. Jamais reposte.</summary>
    public string VehicleDesignation { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le libellé de la réparation est obligatoire.")]
    [StringLength(200, ErrorMessage = "200 caractères maximum.")]
    [Display(Name = "Réparation")]
    public string Description { get; set; } = string.Empty;

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
