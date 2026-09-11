using Microsoft.AspNetCore.Identity;

namespace P5.Data
{
    /// <summary>
    /// Crée le compte du gérant au démarrage. Contrairement à l'inventaire, il ne peut pas
    /// être inséré par une migration : le mot de passe doit être haché à l'exécution par
    /// UserManager.
    /// </summary>
    public static class IdentitySeed
    {
        public static async Task SeedAdminAsync(IServiceProvider services)
        {
            var configuration = services.GetRequiredService<IConfiguration>();
            var email = configuration["AdminAccount:Email"];
            var password = configuration["AdminAccount:Password"];

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                // Sans identifiants, le site reste consultable : seul le back-office est inaccessible.
                services.GetRequiredService<ILogger<ApplicationDbContext>>()
                    .LogWarning("Compte gérant non créé : AdminAccount:Email ou AdminAccount:Password est absent.");
                return;
            }

            var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
            if (await userManager.FindByEmailAsync(email) is not null)
            {
                return;
            }

            var manager = new IdentityUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true // aucun IEmailSender n'est configuré sur ce prototype
            };

            var result = await userManager.CreateAsync(manager, password);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    "Création du compte gérant impossible : " + string.Join(" ", result.Errors.Select(e => e.Description)));
            }
        }
    }
}
