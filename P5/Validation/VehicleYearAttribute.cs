using System.ComponentModel.DataAnnotations;

namespace P5.Validation
{
    /// <summary>
    /// Contrainte métier issue des spécifications : Jacques n'achète jamais de
    /// véhicule antérieur à 1990, et ne doit pas pouvoir saisir 2117 par erreur.
    /// Un attribut dédié est nécessaire car [Range] n'accepte que des constantes,
    /// alors que la borne haute doit suivre l'année courante.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class VehicleYearAttribute : ValidationAttribute
    {
        public const int MinYear = 1990;

        // +1 pour accepter les millésimes anticipés (une "2027" vendue fin 2026).
        public static int MaxYear => DateTime.Today.Year + 1;

        protected override ValidationResult? IsValid(object? value, ValidationContext context)
        {
            // L'absence de valeur relève de [Required], pas de cet attribut.
            if (value is null)
            {
                return ValidationResult.Success;
            }

            if (value is not int year)
            {
                return new ValidationResult("L'année doit être un nombre entier.");
            }

            return year >= MinYear && year <= MaxYear
                ? ValidationResult.Success
                : new ValidationResult(
                    $"L'année doit être comprise entre {MinYear} et {MaxYear}.",
                    [context.MemberName ?? nameof(year)]);
        }
    }
}
