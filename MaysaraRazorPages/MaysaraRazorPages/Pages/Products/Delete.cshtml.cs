using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MaysaraRazorPages.Data;
using MaysaraRazorPages.Models;
using MaysaraRazorPages.Services;

namespace MaysaraRazorPages.Pages.Products
{
    /// <summary>
    /// Product Delete Page - Uses Entity Framework
    /// Demonstrates: LINQ Find, Remove (READ and DELETE operations)
    /// </summary>
    public class DeleteModel : PageModel
    {
        private readonly MaysaraDbContext _context;

        public DeleteModel(MaysaraDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Product? Product { get; set; }

        public string? RestaurantName { get; set; }
        public string? ErrorMessage { get; set; }

        public IActionResult OnGet(int? id)
        {
            var userId = SessionManager.GetUserId(HttpContext);
            var userRole = SessionManager.GetUserRole(HttpContext);

            if (userId == null)
            {
                return RedirectToPage("/Auth/Login");
            }

            if (userRole != "Vendor")
            {
                ErrorMessage = "Only vendors can delete products.";
                return Page();
            }

            if (id == null)
            {
                ErrorMessage = "Invalid product ID.";
                return Page();
            }

            try
            {
                // EF CRUD - READ: Find product by ID
                Product = _context.Products.Find(id.Value);

                if (Product == null)
                {
                    ErrorMessage = "Product not found.";
                    return Page();
                }

                // Load restaurant information
                var restaurant = _context.Restaurants.Find(Product.RestaurantID);

                // Verify the product belongs to vendor's restaurant
                if (restaurant == null || restaurant.OwnerID != userId.Value)
                {
                    ErrorMessage = "You don't have permission to delete this product.";
                    Product = null;
                    return Page();
                }

                RestaurantName = restaurant.Name;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading product: {ex.Message}";
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            var userId = SessionManager.GetUserId(HttpContext);
            var userRole = SessionManager.GetUserRole(HttpContext);

            if (userId == null)
            {
                return RedirectToPage("/Auth/Login");
            }

            if (userRole != "Vendor")
            {
                ErrorMessage = "Only vendors can delete products.";
                return Page();
            }

            try
            {
                // Get product from database
                var productToDelete = _context.Products
                    .Include(p => p.Restaurant)
                    .Where(p => p.ProductID == Product!.ProductID)
                    .FirstOrDefault();

                if (productToDelete == null)
                {
                    ErrorMessage = "Product not found.";
                    return Page();
                }

                // Verify ownership
                if (productToDelete.Restaurant?.OwnerID != userId.Value)
                {
                    ErrorMessage = "You don't have permission to delete this product.";
                    return Page();
                }

                // EF CRUD - DELETE: Remove product
                _context.Products.Remove(productToDelete);
                _context.SaveChanges();

                // Redirect to products list
                return RedirectToPage("/Products/Create");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error deleting product: {ex.Message}";
                return Page();
            }
        }
    }
}
