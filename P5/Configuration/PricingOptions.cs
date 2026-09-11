namespace P5.Configuration
{
    /// <summary>Réglages de tarification, section « Pricing » de la configuration.</summary>
    public class PricingOptions
    {
        public const string SectionName = "Pricing";

        /// <summary>Marge ajoutée au prix de revient de chaque véhicule.</summary>
        public decimal FixedMargin { get; set; } = Models.Vehicle.DefaultMargin;
    }
}
