using Microsoft.AspNetCore.Identity;
using P5.Security;

namespace P5.Data
{
    /// <summary>
    /// Crée le compte du gérant au démarrage, avec un mot de passe haché par UserManager.
    /// Les identifiants viennent de la section de configuration AdminAccount, à surcharger
    /// via les user-secrets ou une variable d'environnement.
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
                // Le compte existe déjà : on vérifie seulement qu'il porte le rôle.
                await EnsureAdminRoleAsync(userManager, existing);
                return;
            }

            var manager = new IdentityUser
            {
                UserName = email,
                Email = email,
                // Le compte est confirmé immédiatement, sans envoi d'email.
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(manager, password!);
            if (!result.Succeeded)
            {
                // Le site démarre même si le mot de passe est rejeté (ex. trop court) :
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
