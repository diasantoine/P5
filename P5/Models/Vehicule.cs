using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using P5.Validation;

namespace P5.Models
{
    /// <summary>
    /// Un véhicule PHYSIQUE du stock : l'exemplaire précis acheté par Jacques,
    /// avec son propre prix d'achat, ses propres réparations et sa propre vente.
    /// À ne pas confondre avec <see cref="ModeleVoiture"/>, qui est une catégorie du catalogue.
    /// </summary>
    public class Vehicule
    {
        /// <summary>Marge fixe ajoutée au prix de revient (spécifications fonctionnelles).</summary>
        public const decimal MargeFixe = 500m;

        public int Id { get; set; }

        [StringLength(17, MinimumLength = 17, ErrorMessage = "Le code VIN comporte exactement 17 caractères.")]
        [Display(Name = "Code VIN")]
        public string? CodeVin { get; set; }

        [AnneeVehicule]
        [Display(Name = "Année")]
        public int Annee { get; set; }

        [Display(Name = "Modèle")]
        public int ModeleVoitureId { get; set; }
        public ModeleVoiture? ModeleVoiture { get; set; }

        // Conservée en texte libre : les 7 lignes de l'inventaire montrent 7 finitions
        // distinctes (LE, Sport, TCe, XLT...) — aucune mutualisation à en tirer.
        [StringLength(30)]
        [Display(Name = "Finition")]
        public string? Finition { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date d'achat")]
        public DateOnly DateAchat { get; set; }

        [Range(0, 1_000_000, ErrorMessage = "Le prix d'achat doit être positif.")]
        [DataType(DataType.Currency)]
        // ASP.NET Core n'a pas de gabarit d'affichage pour DataType.Currency : sans
        // DisplayFormat, DisplayFor rendrait "9900,00" au lieu de "9 900,00 €".
        // ApplyFormatInEditMode reste à false pour que le champ de saisie reste brut.
        [DisplayFormat(DataFormatString = "{0:C}")]
        [Display(Name = "Prix d'achat")]
        public decimal PrixAchat { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Disponible à la vente le")]
        public DateOnly? DateDisponibilite { get; set; }

        /// <summary>Renseignée = véhicule vendu. Nulle = encore disponible.</summary>
        [DataType(DataType.Date)]
        [Display(Name = "Date de vente")]
        public DateOnly? DateVente { get; set; }

        [StringLength(1000)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Description de l'annonce")]
        public string? Description { get; set; }

        [StringLength(260)]
        [Display(Name = "Photo")]
        public string? PhotoUrl { get; set; }

        public ICollection<Reparation> Reparations { get; set; } = [];

        // ---------------------------------------------------------------
        // Propriétés calculées : [NotMapped] = aucune colonne en base.
        // Elles se recalculent toujours à partir des données sources, donc
        // aucun risque d'incohérence si une réparation est ajoutée plus tard.
        // Nécessitent un .Include(v => v.Reparations) pour être justes.
        // ---------------------------------------------------------------

        [NotMapped]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [Display(Name = "Coût des réparations")]
        public decimal CoutReparations => Reparations.Sum(r => r.Cout);

        /// <summary>Prix d'achat + réparations + 500 € (règle métier de Jacques).</summary>
        [NotMapped]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [Display(Name = "Prix de vente")]
        public decimal PrixVente => PrixAchat + CoutReparations + MargeFixe;

        [NotMapped]
        [Display(Name = "Disponible")]
        public bool EstDisponible => DateVente is null;
    }
}
