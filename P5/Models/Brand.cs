using System.ComponentModel.DataAnnotations;

namespace P5.Models
{
    /// <summary>
    /// Référentiel des constructeurs (Ford, Renault...).
    /// Mutualisé : "Ford" n'est stocké qu'une fois, quel que soit le nombre de véhicules.
    /// </summary>
    public class Brand
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom de la marque est obligatoire.")]
        [StringLength(50, ErrorMessage = "50 caractères maximum.")]
        [Display(Name = "Marque")]
        public string Name { get; set; } = string.Empty;

        public ICollection<CarModel> CarModels { get; set; } = [];

        public ICollection<VehicleSpecification> Specifications { get; set; } = [];
    }
}
