using System.ComponentModel.DataAnnotations;

namespace P5.Models
{
    /// <summary>
    /// Une réparation unitaire effectuée sur un véhicule.
    /// </summary>
    public class Repair
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le libellé de la réparation est obligatoire.")]
        [StringLength(200, ErrorMessage = "200 caractères maximum.")]
        [Display(Name = "Réparation")]
        public string Description { get; set; } = string.Empty;

        [Range(0, 100_000, ErrorMessage = "Le coût doit être positif.")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [Display(Name = "Coût")]
        public decimal Cost { get; set; }

        public int VehicleId { get; set; }
        public Vehicle? Vehicle { get; set; }
    }
}
