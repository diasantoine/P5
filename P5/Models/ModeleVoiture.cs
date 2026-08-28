using System.ComponentModel.DataAnnotations;

namespace P5.Models
{
    /// <summary>
    /// Référentiel des modèles du catalogue (Explorer, Civic...).
    /// Nommé "ModeleVoiture" et non "Modele" pour éviter la confusion avec
    /// la notion de "Model" au sens MVC.
    /// </summary>
    public class ModeleVoiture
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom du modèle est obligatoire.")]
        [StringLength(50, ErrorMessage = "50 caractères maximum.")]
        [Display(Name = "Modèle")]
        public string Nom { get; set; } = string.Empty;

        [Display(Name = "Marque")]
        public int MarqueId { get; set; }

        // Nullable : EF ne la remplit que si on demande explicitement l'Include().
        public Marque? Marque { get; set; }

        public ICollection<Vehicule> Vehicules { get; set; } = [];
    }
}
