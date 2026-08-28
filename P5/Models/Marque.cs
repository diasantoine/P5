using System.ComponentModel.DataAnnotations;

namespace P5.Models
{
    /// <summary>
    /// Référentiel des constructeurs (Ford, Renault...).
    /// Mutualisé : "Ford" n'est stocké qu'une fois, quel que soit le nombre de véhicules.
    /// </summary>
    public class Marque
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom de la marque est obligatoire.")]
        [StringLength(50, ErrorMessage = "50 caractères maximum.")]
        [Display(Name = "Marque")]
        public string Nom { get; set; } = string.Empty;

        // Propriété de navigation : les modèles rattachés à cette marque.
        public ICollection<ModeleVoiture> Modeles { get; set; } = [];
    }
}
