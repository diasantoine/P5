using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using P5.Validation;

namespace P5.Models
{
    /// <summary>
    /// Un véhicule PHYSIQUE du stock : l'exemplaire précis acheté par Jacques,
    /// avec son propre prix d'achat, ses propres réparations et sa propre vente.
    /// À ne pas confondre avec <see cref="CarModel"/>, qui est une catégorie du catalogue.
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

        [Display(Name = "Modèle")]
        public int CarModelId { get; set; }
        public CarModel? CarModel { get; set; }

        // Conservée en texte libre : les 7 lignes de l'inventaire montrent 7 finitions
        // distinctes (LE, Sport, TCe, XLT...) — aucune mutualisation à en tirer.
        [StringLength(30)]
        [Display(Name = "Finition")]
        public string? Trim { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date d'achat")]
        public DateOnly PurchaseDate { get; set; }

        [Range(0, 1_000_000, ErrorMessage = "Le prix d'achat doit être positif.")]
        [DataType(DataType.Currency)]
        // ASP.NET Core n'a pas de gabarit d'affichage pour DataType.Currency : sans
        // DisplayFormat, DisplayFor rendrait "9900,00" au lieu de "9 900,00 €".
        // ApplyFormatInEditMode reste à false pour que le champ de saisie reste brut.
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

        // ---------------------------------------------------------------
        // Propriétés calculées : [NotMapped] = aucune colonne en base.
        // Elles se recalculent toujours à partir des données sources, donc
        // aucun risque d'incohérence si une réparation est ajoutée plus tard.
        // Nécessitent un .Include(v => v.Repairs) pour être justes.
        // ---------------------------------------------------------------

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
    }
}
