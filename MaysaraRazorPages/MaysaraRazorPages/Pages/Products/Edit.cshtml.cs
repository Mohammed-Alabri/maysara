using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MaysaraRazorPages.Data;
using MaysaraRazorPages.Models;
using MaysaraRazorPages.Services;

namespace MaysaraRazorPages.Pages.Products
{
    /// <summary>
    /// Product Edit Page - Uses Entity Framework
    /// Demonstrates: LINQ Find, Where, Update (READ and UPDATE operations)
    /// Mixed syntax: Query syntax + Method chaining
    /// </summary>
    public class EditModel : PageModel
    {
        private readonly MaysaraDbContext _context;

        public EditModel(MaysaraDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Product? Product { get; set; }

        public string? RestaurantName { get; set; }
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }

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
                ErrorMessage = "Only vendors can edit products.";
                return Page();
            }

            if (id == null)
            {
                ErrorMessage = "Invalid product ID.";
                return Page();
            }

            try
            {
                // EF CRUD - READ: Mixed syntax (query syntax + method chaining)
                Product = (from p in _context.Products
                           where p.ProductID == id.Value
                           select p)
                          .Include(p => p.Restaurant)
                          .FirstOrDefault();

                // Alternative: Method syntax with Find (commented)
                // Product = _context.Products.Find(id.Value);

                if (Product == null)
                {
                    ErrorMessage = "Product not found.";
                    return Page();
                }

                // Verify the product belongs to vendor's restaurant
                if (Product.Restaurant?.OwnerID != userId.Value)
                {
                    ErrorMessage = "You don't have permission to edit this product.";
                    Product = null;
                    return Page();
                }

                RestaurantName = Product.Restaurant?.Name ?? "Unknown";
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
                ErrorMessage = "Only vendors can edit products.";
                return Page();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // Get existing product from database
                var existingProduct = _context.Products
                    .Include(p => p.Restaurant)
                    .Where(p => p.ProductID == Product!.ProductID)
                    .FirstOrDefault();

                if (existingProduct == null)
                {
                    ErrorMessage = "Product not found.";
                    return Page();
                }

                // Verify ownership
                if (existingProduct.Restaurant?.OwnerID != userId.Value)
                {
                    ErrorMessage = "You don't have permission to edit this product.";
                    return Page();
                }

                // EF CRUD - UPDATE: Update properties
                existingProduct.Name = Product!.Name;
                existingProduct.Description = Product.Description;
                existingProduct.Price = Product.Price;
                existingProduct.Category = Product.Category;
                existingProduct.Stock = Product.Stock;
                existingProduct.IsAvailable = Product.IsAvailable;

                _context.Products.Update(existingProduct);
                _context.SaveChanges();

                SuccessMessage = "Product updated successfully!";

                // Reload restaurant name for display
                RestaurantName = existingProduct.Restaurant?.Name ?? "Unknown";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error updating product: {ex.Message}";
            }

            return Page();
        }
    }
}
