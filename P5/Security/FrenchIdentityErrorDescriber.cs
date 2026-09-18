using Microsoft.AspNetCore.Identity;

namespace P5.Security
{
    /// <summary>
    /// Identity produit ses messages d'erreur en anglais. On ne surcharge que ceux qu'un visiteur
    /// peut réellement rencontrer en s'inscrivant ; les autres, internes, gardent leur texte d'origine.
    /// La politique de mot de passe n'est pas touchée : on traduit les refus, on ne les affaiblit pas.
    /// </summary>
    public class FrenchIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError DuplicateEmail(string email) => new()
        {
            Code = nameof(DuplicateEmail),
            Description = "Cette adresse email est déjà utilisée."
        };

        // Le nom d'utilisateur est l'adresse email : le même message évite d'en afficher deux différents.
        public override IdentityError DuplicateUserName(string userName) => new()
        {
            Code = nameof(DuplicateUserName),
            Description = "Cette adresse email est déjà utilisée."
        };

        public override IdentityError InvalidEmail(string? email) => new()
        {
            Code = nameof(InvalidEmail),
            Description = "Cette adresse email n'est pas valide."
        };

        public override IdentityError PasswordTooShort(int length) => new()
        {
            Code = nameof(PasswordTooShort),
            Description = $"Le mot de passe doit comporter au moins {length} caractères."
        };

        public override IdentityError PasswordRequiresDigit() => new()
        {
            Code = nameof(PasswordRequiresDigit),
            Description = "Le mot de passe doit contenir au moins un chiffre."
        };

        public override IdentityError PasswordRequiresLower() => new()
        {
            Code = nameof(PasswordRequiresLower),
            Description = "Le mot de passe doit contenir au moins une minuscule."
        };

        public override IdentityError PasswordRequiresUpper() => new()
        {
            Code = nameof(PasswordRequiresUpper),
            Description = "Le mot de passe doit contenir au moins une majuscule."
        };

        public override IdentityError PasswordRequiresNonAlphanumeric() => new()
        {
            Code = nameof(PasswordRequiresNonAlphanumeric),
            Description = "Le mot de passe doit contenir au moins un caractère spécial."
        };
    }
}
