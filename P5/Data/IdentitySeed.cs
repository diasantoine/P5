using Microsoft.AspNetCore.Identity;
using P5.Security;

namespace P5.Data
{
    /// <summary>
    /// Crée le compte du gérant au démarrage. Contrairement à l'inventaire, il ne peut pas
    /// être inséré par une migration : le mot de passe doit être haché à l'exécution par
    /// UserManager.
    /// Les identifiants viennent de la section de configuration AdminAccount ; ceux commités
    /// dans appsettings.json sont des identifiants de démonstration, admis pour un prototype
    /// jamais déployé, et à surcharger via les user-secrets ou une variable d'environnement.
    /// </summary>
    public static class IdentitySeed
    {
        public static async Task SeedAdminAsync(IServiceProvider services)
        {
            var configuration = services.GetRequiredService<IConfiguration>();
            var email = configuration["AdminAccount:Email"];
            var password = configuration["AdminAccount:Password"];

            var missing = new List<string>();
            if (string.IsNullOrWhiteSpace(email))
            {
                missing.Add("AdminAccount:Email");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                missing.Add("AdminAccount:Password");
            }

            if (missing.Count > 0)
            {
                // Sans identifiants, le site reste consultable : seul le back-office est inaccessible.
                services.GetRequiredService<ILoggerFactory>()
                    .CreateLogger("P5.Data.IdentitySeed")
                    .LogWarning("Compte gérant non créé : {MissingKeys} absent de la configuration.",
                        string.Join(" et ", missing));
                return;
            }

            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            if (!await roleManager.RoleExistsAsync(AppRoles.Admin))
            {
                await roleManager.CreateAsync(new IdentityRole(AppRoles.Admin));
            }

            var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
            var existing = await userManager.FindByEmailAsync(email!);
            if (existing is not null)
            {
                // Une base créée avant l'ouverture de l'inscription contient déjà le compte, sans rôle.
                await EnsureAdminRoleAsync(userManager, existing);
                return;
            }

            var manager = new IdentityUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true // aucun IEmailSender n'est configuré sur ce prototype
            };

            var result = await userManager.CreateAsync(manager, password!);
            if (!result.Succeeded)
            {
                // Un mot de passe rejete (ex. trop court) ne doit pas empecher le site de demarrer :
                // comme pour des identifiants absents, seul le back-office reste inaccessible.
                services.GetRequiredService<ILoggerFactory>()
                    .CreateLogger("P5.Data.IdentitySeed")
                    .LogError("Compte gérant non créé : {Errors}",
                        string.Join(" ", result.Errors.Select(e => e.Description)));
                return;
            }

            await EnsureAdminRoleAsync(userManager, manager);
        }

        private static async Task EnsureAdminRoleAsync(UserManager<IdentityUser> userManager, IdentityUser user)
        {
            if (!await userManager.IsInRoleAsync(user, AppRoles.Admin))
            {
                await userManager.AddToRoleAsync(user, AppRoles.Admin);
            }
        }
    }
}
