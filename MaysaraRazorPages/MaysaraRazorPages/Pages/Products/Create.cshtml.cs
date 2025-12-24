using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MaysaraRazorPages.Data;
using MaysaraRazorPages.Models;
using MaysaraRazorPages.Services;

namespace MaysaraRazorPages.Pages.Products
{
    /// <summary>
    /// Product Create Page - Uses Entity Framework
    /// Demonstrates: LINQ Where, ToList, Include, Add (CREATE operation)
    /// </summary>
    public class CreateModel : PageModel
    {
        private readonly MaysaraDbContext _context;

        public CreateModel(MaysaraDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Product NewProduct { get; set; } = new Product();

        public List<Product>? VendorProducts { get; set; }
        public SelectList? RestaurantList { get; set; }
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }

        public IActionResult OnGet()
        {
            var userId = SessionManager.GetUserId(HttpContext);
            var userRole = SessionManager.GetUserRole(HttpContext);

            if (userId == null)
            {
                return RedirectToPage("/Auth/Login");
            }

            // Only Vendors can create products
            if (userRole != "Vendor")
            {
                ErrorMessage = "Only vendors can create products.";
                return Page();
            }

            try
            {
                // LINQ: Method syntax with Where and ToList - Get vendor's restaurants
                var vendorRestaurants = _context.Restaurants
                    .Where(r => r.OwnerID == userId.Value)
                    .ToList();

                RestaurantList = new SelectList(vendorRestaurants, "RestaurantID", "Name");

                // LINQ: Get all products for vendor's restaurants with Include (eager loading)
                VendorProducts = _context.Products
                    .Include(p => p.Restaurant)
                    .Where(p => p.Restaurant!.OwnerID == userId.Value)
                    .OrderBy(p => p.Category)
                    .ThenBy(p => p.Name)
                    .ToList();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading data: {ex.Message}";
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
                ErrorMessage = "Only vendors can create products.";
                return Page();
            }

            if (!ModelState.IsValid)
            {
                // Reload restaurant list for dropdown
                var vendorRestaurants = _context.Restaurants
                    .Where(r => r.OwnerID == userId.Value)
                    .ToList();
                RestaurantList = new SelectList(vendorRestaurants, "RestaurantID", "Name");

                return Page();
            }

            try
            {
                // Verify the restaurant belongs to the logged-in vendor
                var restaurant = _context.Restaurants
                    .Where(r => r.RestaurantID == NewProduct.RestaurantID && r.OwnerID == userId.Value)
                    .FirstOrDefault();

                if (restaurant == null)
                {
                    ErrorMessage = "Invalid restaurant selection. You can only add products to your own restaurants.";

                    // Reload data
                    var vendorRestaurants = _context.Restaurants
                        .Where(r => r.OwnerID == userId.Value)
                        .ToList();
                    RestaurantList = new SelectList(vendorRestaurants, "RestaurantID", "Name");

                    return Page();
                }

                // Set creation date
                NewProduct.CreatedAt = DateTime.Now;

                // EF CRUD - CREATE: Add new product
                _context.Products.Add(NewProduct);
                _context.SaveChanges();

                SuccessMessage = "Product added successfully!";

                // Redirect to same page to show updated list
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error adding product: {ex.Message}";

                // Reload restaurant list
                var vendorRestaurants = _context.Restaurants
                    .Where(r => r.OwnerID == userId.Value)
                    .ToList();
                RestaurantList = new SelectList(vendorRestaurants, "RestaurantID", "Name");

                return Page();
            }
        }
    }
}
