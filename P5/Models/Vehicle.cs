using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using P5.Validation;

namespace P5.Models
{
    /// <summary>
    /// Un véhicule PHYSIQUE du stock : l'exemplaire précis acheté par Jacques,
    /// avec son propre prix d'achat, ses propres réparations et sa propre vente.
    /// À ne pas confondre avec <see cref="CarModel"/> ou <see cref="Trim"/>, qui sont
    /// des catégories du catalogue.
    /// </summary>
    public class Vehicle
    {
        /// <summary>Marge fixe ajoutée au prix de revient (spécifications fonctionnelles).</summary>
        public const decimal FixedMargin = 500m;

        public int Id { get; set; }

        [StringLength(17, MinimumLength = 17, ErrorMessage = "Le code VIN comporte exactement 17 caractères.")]
        [Display(Name = "Code VIN")]
        public string? Vin { get; set; }

        [VehicleYear]
        [Display(Name = "Année")]
        public int Year { get; set; }

        // Le véhicule ne référence que sa finition : la marque et le modèle se
        // déduisent par la chaîne Vehicle -> Trim -> CarModel -> Brand. Une seule
        // clé étrangère, donc aucune incohérence possible entre modèle et finition.
        [Display(Name = "Modèle et finition")]
        public int TrimId { get; set; }
        public Trim? Trim { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date d'achat")]
        public DateOnly PurchaseDate { get; set; }

        [Range(0, 1_000_000, ErrorMessage = "Le prix d'achat doit être positif.")]
        [DataType(DataType.Currency)]
        // ASP.NET Core n'a pas de gabarit d'affichage pour DataType.Currency.
        // ApplyFormatInEditMode reste à false : le champ de saisie doit rester brut.
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

        // Calculées à la lecture depuis Repairs : un .Include(v => v.Repairs) est indispensable, sinon RepairsCost vaut 0.

        [NotMapped]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [Display(Name = "Coût des réparations")]
        public decimal RepairsCost => Repairs.Sum(r => r.Cost);

        /// <summary>Prix d'achat + réparations + 500 € (règle métier de Jacques).</summary>
        [NotMapped]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [Display(Name = "Prix de vente")]
        public decimal SalePrice => PurchasePrice + RepairsCost + FixedMargin;

        [NotMapped]
        [Display(Name = "Disponible")]
        public bool IsAvailable => SaleDate is null;

        // Necessite le chargement de la chaine Trim -> CarModel -> Brand (cf. WithDependencies dans VehicleService).
        [NotMapped]
        [Display(Name = "Véhicule")]
        public string Designation => string.Join(' ', new[] { Trim?.CarModel?.Brand?.Name, Trim?.CarModel?.Name, Trim?.Name }.Where(s => !string.IsNullOrWhiteSpace(s)));
    }
}
