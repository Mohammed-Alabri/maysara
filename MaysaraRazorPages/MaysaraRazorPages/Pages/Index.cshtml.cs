using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MaysaraRazorPages.Pages
{
    public class IndexModel : PageModel
    {
        public IActionResult OnGet()
        {
            // Redirect to restaurants page
            return RedirectToPage("/Restaurants/Index");
        }
    }
}
