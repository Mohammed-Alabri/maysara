using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MaysaraRazorPages.Data;
using MaysaraRazorPages.Models;
using MaysaraRazorPages.Models.Enums;
using MaysaraRazorPages.Services;
using MaysaraRazorPages.Helpers;

namespace MaysaraRazorPages.Pages.Orders
{
    /// <summary>
    /// Checkout Page - Uses Entity Framework
    /// Demonstrates: LINQ Where, Contains, Sum, AddRange, Transactions
    /// EF CRUD - CREATE: Create Order and OrderItems
    /// </summary>
    public class CheckoutModel : PageModel
    {
        private readonly MaysaraDbContext _context;

        public CheckoutModel(MaysaraDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Order NewOrder { get; set; } = new Order();

        public SelectList? RestaurantList { get; set; }
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }

        public IActionResult OnGet()
        {
            var userId = SessionManager.GetUserId(HttpContext);

            if (userId == null)
            {
                return RedirectToPage("/Auth/Login");
            }

            // Check if cart exists
            var cart = SessionManager.GetCart(HttpContext);
            if (cart == null || cart.Items.Count == 0)
            {
                TempData["ErrorMessage"] = "Your cart is empty. Please add items before checkout.";
                return RedirectToPage("/Orders/Cart");
            }

            // Pre-populate restaurant from cart
            NewOrder.RestaurantID = cart.RestaurantID;

            LoadData(userId.Value);
            return Page();
        }

        public IActionResult OnPost()
        {
            var userId = SessionManager.GetUserId(HttpContext);
            var userEmail = SessionManager.GetUserEmail(HttpContext);

            if (userId == null)
            {
                return RedirectToPage("/Auth/Login");
            }

            // Get cart from session
            var cart = SessionManager.GetCart(HttpContext);
            if (cart == null || cart.Items.Count == 0)
            {
                ErrorMessage = "Your cart is empty.";
                return RedirectToPage("/Orders/Cart");
            }

            // Validate cart restaurant matches order
            if (cart.RestaurantID != NewOrder.RestaurantID)
            {
                ErrorMessage = "Cart restaurant does not match selected restaurant.";
                LoadData(userId.Value);
                return Page();
            }

            // Remove validation for OrderItems and TotalAmount (calculated server-side, not from form)
            ModelState.Remove("NewOrder.OrderItems");
            ModelState.Remove("NewOrder.TotalAmount");

            if (!ModelState.IsValid)
            {
                ErrorMessage = "Please fill in all required fields.";
                LoadData(userId.Value);
                return Page();
            }

            try
            {
                // Convert cart items to order items
                var orderItems = new List<OrderItem>();
                var productIds = cart.Items.Select(i => i.ProductID).ToList();

                // Verify products still exist and are available
                var products = _context.Products
                    .Where(p => productIds.Contains(p.ProductID) && p.IsAvailable)
                    .ToList();

                if (products.Count != productIds.Count)
                {
                    ErrorMessage = "Some products in your cart are no longer available.";
                    LoadData(userId.Value);
                    return Page();
                }

                // Create order items from cart
                foreach (var cartItem in cart.Items)
                {
                    var product = products.FirstOrDefault(p => p.ProductID == cartItem.ProductID);
                    if (product == null || product.Stock < cartItem.Quantity)
                    {
                        ErrorMessage = $"Insufficient stock for {cartItem.ProductName}.";
                        LoadData(userId.Value);
                        return Page();
                    }

                    orderItems.Add(new OrderItem
                    {
                        ProductID = cartItem.ProductID,
                        ProductName = cartItem.ProductName,
                        UnitPrice = cartItem.Price,
                        Quantity = cartItem.Quantity
                    });
                }

                // Calculate total
                decimal totalAmount = orderItems.Sum(item => item.UnitPrice * item.Quantity);

                // Create order WITHOUT OrderItems to avoid EF tracking issues
                NewOrder.UserID = userId.Value;
                NewOrder.TotalAmount = totalAmount;
                NewOrder.Status = OrderStatus.Pending;
                NewOrder.OrderDate = DateTime.Now;

                // EF Transaction
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        // Add and save Order to get OrderID
                        _context.Orders.Add(NewOrder);
                        _context.SaveChanges();

                        // NOW set OrderID for each item
                        foreach (var item in orderItems)
                        {
                            item.OrderID = NewOrder.OrderID;
                        }

                        // Add OrderItems and save
                        _context.OrderItems.AddRange(orderItems);
                        _context.SaveChanges();

                        transaction.Commit();

                        // Clear cart after successful order
                        SessionManager.ClearCart(HttpContext);

                        SuccessMessage = $"Order #{NewOrder.OrderID} placed successfully! Total: {CurrencyHelper.FormatCurrency(totalAmount)}";
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception($"Transaction failed: {ex.Message}", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error creating order: {ex.Message}";
                LoadData(userId.Value);
                return Page();
            }

            return Page();
        }

        private void LoadData(int userId)
        {
            try
            {
                // LINQ: Get active restaurants for dropdown
                var restaurants = _context.Restaurants
                    .Where(r => r.IsActive)
                    .OrderBy(r => r.Name)
                    .ToList();

                RestaurantList = new SelectList(restaurants, "RestaurantID", "Name");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading data: {ex.Message}";
            }
        }
    }
}
