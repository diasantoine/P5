using System.ComponentModel.DataAnnotations;

namespace P5.Models
{
    /// <summary>
    /// Référentiel des finitions du catalogue (LE, XLT, Sport...), troisième niveau
    /// de la hiérarchie Marque > Modèle > Finition.
    /// Une finition n'a de sens que pour un modèle donné : « LE » chez Mazda n'a rien
    /// à voir avec « LE » chez un autre constructeur, d'où le rattachement à
    /// <see cref="CarModel"/> et non une table mondiale de libellés.
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

        // Nullable : EF ne la remplit que si on demande explicitement l'Include().
        public CarModel? CarModel { get; set; }

        public ICollection<Vehicle> Vehicles { get; set; } = [];
    }
}
