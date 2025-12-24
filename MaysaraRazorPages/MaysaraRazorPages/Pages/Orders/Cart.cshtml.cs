using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MaysaraRazorPages.Models;
using MaysaraRazorPages.Services;

namespace MaysaraRazorPages.Pages.Orders
{
    /// <summary>
    /// Shopping Cart Page - View and manage cart items
    /// </summary>
    public class CartModel : PageModel
    {
        public ShoppingCart? Cart { get; set; }
        public string? ErrorMessage { get; set; }

        public IActionResult OnGet()
        {
            var userId = SessionManager.GetUserId(HttpContext);
            if (userId == null)
            {
                return RedirectToPage("/Auth/Login");
            }

            Cart = SessionManager.GetCart(HttpContext);
            return Page();
        }

        public IActionResult OnPostUpdate(int productId, int quantity)
        {
            SessionManager.UpdateCartItem(HttpContext, productId, quantity);
            return RedirectToPage();
        }

        public IActionResult OnPostRemove(int productId)
        {
            SessionManager.UpdateCartItem(HttpContext, productId, 0);
            return RedirectToPage();
        }

        public IActionResult OnPostClear()
        {
            SessionManager.ClearCart(HttpContext);
            return RedirectToPage();
        }
    }
}
