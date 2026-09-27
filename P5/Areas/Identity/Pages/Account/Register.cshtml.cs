using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace P5.Areas.Identity.Pages.Account
{
    /// <summary>
    /// Remplace la page d'inscription d'Identity : le site n'a qu'un compte, celui du gérant,
    /// créé au démarrage. Une page portée par le projet l'emporte sur celle de la bibliothèque
    /// Microsoft.AspNetCore.Identity.UI. Le code de l'inscription ouverte est conservé en
    /// commentaire ci-dessous, pour une évolution éventuelle.
    /// </summary>
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        public IActionResult OnGet() => NotFound();

        public IActionResult OnPost() => NotFound();
    }

    /*
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Identity;

    /// <summary>
    /// Page d'inscription en français. Un compte créé ici ne reçoit aucun rôle et consulte le
    /// site comme un visiteur ; seul le compte du gérant, créé au démarrage, a le rôle Admin.
    /// </summary>
    [AllowAnonymous]
    public class RegisterModel(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager) : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager = userManager;
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
            [StringLength(100, MinimumLength = 6, ErrorMessage = "Le mot de passe doit contenir entre {2} et {1} caractères.")]
            [DataType(DataType.Password)]
            [Display(Name = "Mot de passe")]
            public string Password { get; set; } = string.Empty;

            [DataType(DataType.Password)]
            [Display(Name = "Confirmer le mot de passe")]
            [Compare(nameof(Password), ErrorMessage = "Les deux mots de passe ne correspondent pas.")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        public void OnGet(string? returnUrl = null) => ReturnUrl = returnUrl;

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = new IdentityUser
            {
                UserName = Input.Email,
                Email = Input.Email,
                // Le compte est confirmé immédiatement, sans envoi d'email.
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, Input.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return Page();
            }

            await _signInManager.SignInAsync(user, isPersistent: false);

            // Redirige uniquement vers une adresse du site.
            return LocalRedirect(Url.IsLocalUrl(returnUrl) ? returnUrl! : Url.Content("~/"));
        }
    }
    */
}
