using System.ComponentModel.DataAnnotations;

namespace P5.Models
{
    /// <summary>
    /// Référentiel des finitions du catalogue (LE, XLT, Sport...), troisième niveau
    /// de la hiérarchie Marque > Modèle > Finition, rattachées à un <see cref="CarModel"/> :
    /// « LE » chez Mazda est distincte de « LE » chez un autre constructeur.
    /// </summary>
    public class Trim
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom de la finition est obligatoire.")]
        [StringLength(30, ErrorMessage = "30 caractères maximum.")]
        [Display(Name = "Finition")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Modèle")]
        public int CarModelId { get; set; }

        public CarModel? CarModel { get; set; }

        public ICollection<VehicleSpecification> Specifications { get; set; } = [];
    }
}
