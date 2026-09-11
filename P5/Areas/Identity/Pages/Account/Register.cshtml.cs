using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace P5.Areas.Identity.Pages.Account
{
    /// <summary>
    /// Remplace la page d'inscription d'Identity : le site n'a qu'un compte, celui du gérant,
    /// créé au démarrage, donc la page redirige vers l'écran de connexion en GET et refuse
    /// toute soumission en POST.
    /// </summary>
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        public IActionResult OnGet() => RedirectToPage("/Account/Login");

        public IActionResult OnPost() => NotFound();
    }
}
