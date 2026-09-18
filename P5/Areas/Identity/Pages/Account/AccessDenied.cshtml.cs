using Microsoft.AspNetCore.Mvc.RazorPages;

namespace P5.Areas.Identity.Pages.Account
{
    /// <summary>
    /// Page affichée, en français, à un inscrit sans le rôle Admin qui tente d'atteindre le back-office.
    /// </summary>
    public class AccessDeniedModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
