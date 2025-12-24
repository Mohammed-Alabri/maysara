using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MaysaraRazorPages.Services;

namespace MaysaraRazorPages.Pages.Auth
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            // Clear user session
            SessionManager.ClearSession(HttpContext);

            // Redirect to login page
            return RedirectToPage("/Auth/Login");
        }

        public IActionResult OnPost()
        {
            // Clear user session
            SessionManager.ClearSession(HttpContext);

            // Redirect to login page
            return RedirectToPage("/Auth/Login");
        }
    }
}
