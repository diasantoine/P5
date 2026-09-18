using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P5.Models
{
    /// <summary>
    /// Une entrée du catalogue : le triplet marque + modèle + finition qu'un véhicule
    /// référence par une seule clé étrangère. Chacune des trois caractéristiques est
    /// accessible en une seule jointure depuis cette table.
    /// </summary>
    public class VehicleSpecification
    {
        public int Id { get; set; }

        [Display(Name = "Marque")]
        public int BrandId { get; set; }
        public Brand? Brand { get; set; }

        [Display(Name = "Modèle")]
        public int CarModelId { get; set; }
        public CarModel? CarModel { get; set; }

        [Display(Name = "Finition")]
        public int TrimId { get; set; }
        public Trim? Trim { get; set; }

        public ICollection<Vehicle> Vehicles { get; set; } = [];

        /// <summary>« Ford Edge SEL ». Exige que les trois caractéristiques soient chargées.</summary>
        [NotMapped]
        public string Label => string.Join(' ', new[] { Brand?.Name, CarModel?.Name, Trim?.Name }
            .Where(n => !string.IsNullOrWhiteSpace(n)));
    }
}
