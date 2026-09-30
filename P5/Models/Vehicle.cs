using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using P5.Validation;

namespace P5.Models
{
    /// <summary>
    /// Un véhicule physique du stock, avec son propre prix d'achat, ses propres
    /// réparations et sa propre vente. À ne pas confondre avec <see cref="CarModel"/>
    /// ou <see cref="Trim"/>, qui sont des catégories du catalogue.
    /// </summary>
    public class Vehicle
    {
        /// <summary>Marge par défaut, appliquée tant que la configuration n'en impose pas d'autre.</summary>
        public const decimal DefaultMargin = 500m;

        public int Id { get; set; }

        [StringLength(17, MinimumLength = 17, ErrorMessage = "Le code VIN comporte exactement 17 caractères.")]
        [Display(Name = "Code VIN")]
        public string? Vin { get; set; }

        [VehicleYear]
        [Display(Name = "Année")]
        public int Year { get; set; }

        // La finition porte le modèle, qui porte la marque.
        [Display(Name = "Finition")]
        public int TrimId { get; set; }
        public Trim? Trim { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date d'achat")]
        public DateOnly PurchaseDate { get; set; }

        [Range(0, 1_000_000, ErrorMessage = "Le prix d'achat doit être positif.")]
        [DataType(DataType.Currency)]
        // Affiché au format monétaire ; le champ de saisie garde la valeur brute.
        [DisplayFormat(DataFormatString = "{0:C}")]
        [Display(Name = "Prix d'achat")]
        public decimal PurchasePrice { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Disponible à la vente le")]
        public DateOnly? AvailabilityDate { get; set; }

        /// <summary>Renseignée = véhicule vendu. Nulle = encore disponible.</summary>
        [DataType(DataType.Date)]
        [Display(Name = "Date de vente")]
        public DateOnly? SaleDate { get; set; }

        [StringLength(1000)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Description de l'annonce")]
        public string? Description { get; set; }

        [StringLength(260)]
        [Display(Name = "Photo")]
        public string? PhotoUrl { get; set; }

        public ICollection<Repair> Repairs { get; set; } = [];

        // Calculées à la lecture depuis Repairs, chargées via .Include(v => v.Repairs).

        [NotMapped]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [Display(Name = "Coût des réparations")]
        public decimal RepairsCost => Repairs.Sum(r => r.Cost);

        /// <summary>
        /// Marge appliquée à cet exemplaire, renseignée par le service depuis la configuration.
        /// </summary>
        [NotMapped]
        public decimal Margin { get; set; } = DefaultMargin;

        /// <summary>Prix d'achat + réparations + marge.</summary>
        [NotMapped]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [Display(Name = "Prix de vente")]
        public decimal SalePrice => PurchasePrice + RepairsCost + Margin;

        [NotMapped]
        [Display(Name = "Disponible")]
        public bool IsAvailable => SaleDate is null;

        [NotMapped]
        [Display(Name = "Véhicule")]
        public string Designation => string.Join(' ', new[] { Trim?.CarModel?.Brand?.Name, Trim?.CarModel?.Name, Trim?.Name }
            .Where(n => !string.IsNullOrWhiteSpace(n)));
    }
}
