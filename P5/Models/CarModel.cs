using System.ComponentModel.DataAnnotations;

namespace P5.Models
{
    /// <summary>
    /// Référentiel des modèles du catalogue (Explorer, Civic...).
    /// Nommé "CarModel" et non "Model" pour éviter la confusion avec
    /// la notion de "Model" au sens MVC.
    /// </summary>
    public class CarModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom du modèle est obligatoire.")]
        [StringLength(50, ErrorMessage = "50 caractères maximum.")]
        [Display(Name = "Modèle")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Marque")]
        public int BrandId { get; set; }

        public Brand? Brand { get; set; }

        // Les véhicules ne sont pas rattachés directement au modèle : ils le sont
        // à une finition, qui elle-même appartient au modèle.
        public ICollection<Trim> Trims { get; set; } = [];
    }
}
