using Microsoft.AspNetCore.Identity;

namespace P5.Security
{
    /// <summary>
    /// Traduit en français les messages d'erreur d'Identity qu'un visiteur peut rencontrer ;
    /// les autres gardent leur texte d'origine.
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
