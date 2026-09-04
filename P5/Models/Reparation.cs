using System.ComponentModel.DataAnnotations;

namespace P5.Models
{
    /// <summary>
    /// Une réparation unitaire effectuée sur un véhicule.
    /// Entité séparée car la feuille de calcul entassait plusieurs réparations
    /// dans une seule cellule ("Pneus, freins, climatisation") : non atomique.
    /// </summary>
    public class Reparation
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le libellé de la réparation est obligatoire.")]
        [StringLength(200, ErrorMessage = "200 caractères maximum.")]
        [Display(Name = "Réparation")]
        public string Libelle { get; set; } = string.Empty;

        [Range(0, 100_000, ErrorMessage = "Le coût doit être positif.")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [Display(Name = "Coût")]
        public decimal Cout { get; set; }

        public int VehiculeId { get; set; }
        public Vehicule? Vehicule { get; set; }
    }
}
