using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace P5.Areas.Identity.Pages.Account
{
    /// <summary>
    /// Remplace la page d'inscription d'Identity : le site n'a qu'un compte, celui du
    /// gérant, créé au démarrage. Une page portée par le projet l'emporte sur celle de la
    /// bibliothèque Microsoft.AspNetCore.Identity.UI.
    /// </summary>
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        public IActionResult OnGet() => NotFound();

        public IActionResult OnPost() => NotFound();
    }
}
