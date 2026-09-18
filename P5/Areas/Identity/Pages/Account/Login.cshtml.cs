using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace P5.Areas.Identity.Pages.Account
{
    /// <summary>
    /// Page de connexion, en français.
    /// </summary>
    [AllowAnonymous]
    public class LoginModel(SignInManager<IdentityUser> signInManager) : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager = signInManager;

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public string? ReturnUrl { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "L'adresse email est obligatoire.")]
            [EmailAddress(ErrorMessage = "L'adresse email n'est pas valide.")]
            [Display(Name = "Adresse email")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Le mot de passe est obligatoire.")]
            [DataType(DataType.Password)]
            [Display(Name = "Mot de passe")]
            public string Password { get; set; } = string.Empty;

            [Display(Name = "Rester connecté")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string? returnUrl = null)
        {
            // Déconnecte un éventuel cookie de connexion externe ouvert avant l'affichage de cette page.
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // lockoutOnFailure : le compte se verrouille quelques minutes après plusieurs échecs.
            var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                // LocalRedirect refuse une adresse externe : pas de redirection ouverte après la connexion.
                return LocalRedirect(Url.IsLocalUrl(returnUrl) ? returnUrl! : Url.Content("~/"));
            }

            // Le même message que l'adresse soit inconnue ou le mot de passe faux : on ne révèle pas quels comptes existent.
            ModelState.AddModelError(string.Empty, result.IsLockedOut
                ? "Trop de tentatives : ce compte est verrouillé quelques minutes."
                : "Adresse email ou mot de passe incorrect.");
            return Page();
        }
    }
}
