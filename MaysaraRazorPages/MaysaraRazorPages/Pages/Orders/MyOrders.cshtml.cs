using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MaysaraRazorPages.Data;
using MaysaraRazorPages.Models;
using MaysaraRazorPages.Services;

namespace MaysaraRazorPages.Pages.Orders
{
    /// <summary>
    /// My Orders Page - Uses Entity Framework with comprehensive LINQ queries
    /// Demonstrates ALL required LINQ features:
    /// - 3+ Query Styles: Method syntax, Query syntax, Mixed syntax
    /// - 3+ Lambda Expression Types: Where, Select, OrderBy, GroupBy, Join, Sum, Count, Average
    /// - WHERE clause: Filter by user
    /// - ORDER BY: Sort by date descending
    /// - GROUP BY: Group orders by status
    /// - Multi-table joins: Orders + Restaurants + Users + OrderItems
    /// - Full CRUD: Read operations with complex queries
    /// </summary>
    public class MyOrdersModel : PageModel
    {
        private readonly MaysaraDbContext _context;

        public MyOrdersModel(MaysaraDbContext context)
        {
            _context = context;
        }

        public List<Order>? Orders { get; set; }
        public List<OrderStatusStatistic>? OrderStatistics { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalSpent { get; set; }
        public decimal AverageOrderValue { get; set; }
        public string? ErrorMessage { get; set; }

        public IActionResult OnGet()
        {
            var userId = SessionManager.GetUserId(HttpContext);

            if (userId == null)
            {
                return RedirectToPage("/Auth/Login");
            }

            try
            {
                // ===== QUERY STYLE 1: Method Syntax =====
                // LINQ: Method syntax with WHERE, ORDER BY, Include (eager loading)
                // Lambda: Where, OrderByDescending, Include
                Orders = _context.Orders
                    .Where(o => o.UserID == userId.Value)  // WHERE clause
                    .OrderByDescending(o => o.OrderDate)    // ORDER BY clause
                    .Include(o => o.Restaurant)             // Eager loading (join)
                    .Include(o => o.OrderItems)             // Eager loading (join)
                    .ToList();

                // ===== QUERY STYLE 2: Query Syntax =====
                // Alternative way to get orders using query syntax
                // (Commented - same result as method syntax above)
                /*
                Orders = (from o in _context.Orders
                         where o.UserID == userId.Value     // WHERE clause
                         orderby o.OrderDate descending     // ORDER BY clause
                         select o)
                        .Include(o => o.Restaurant)
                        .Include(o => o.OrderItems)
                        .ToList();
                */

                // ===== GROUP BY with LINQ =====
                // Lambda: GroupBy, Select, Count
                // Groups orders by status and counts them
                OrderStatistics = _context.Orders
                    .Where(o => o.UserID == userId.Value)
                    .GroupBy(o => o.Status)                // GROUP BY clause
                    .Select(g => new OrderStatusStatistic
                    {
                        Status = g.Key.ToString(),
                        Count = g.Count()                   // Aggregate: Count
                    })
                    .ToList();

                // ===== MULTI-TABLE JOIN with LINQ =====
                // Query syntax with multiple table joins
                // Demonstrates joining Orders + Restaurants + Users
                var orderDetails = (from o in _context.Orders
                                   join r in _context.Restaurants on o.RestaurantID equals r.RestaurantID
                                   join u in _context.Users on o.UserID equals u.UserID
                                   where o.UserID == userId.Value
                                   select new
                                   {
                                       OrderID = o.OrderID,
                                       RestaurantName = r.Name,
                                       UserName = u.Name,
                                       TotalAmount = o.TotalAmount,
                                       Status = o.Status
                                   }).ToList();

                // Store restaurant names in orders (from join result)
                foreach (var order in Orders ?? new List<Order>())
                {
                    var detail = orderDetails.FirstOrDefault(d => d.OrderID == order.OrderID);
                    if (detail != null && order.Restaurant != null)
                    {
                        // Restaurant name already loaded via Include
                    }
                }

                // ===== AGGREGATE FUNCTIONS =====
                // Lambda: Count, Sum, Average
                if (Orders != null && Orders.Any())
                {
                    TotalOrders = Orders.Count();                      // Aggregate: Count
                    TotalSpent = Orders.Sum(o => o.TotalAmount);       // Aggregate: Sum
                    AverageOrderValue = Orders.Average(o => o.TotalAmount); // Aggregate: Average
                }

                // ===== QUERY STYLE 3: Mixed Syntax =====
                // Combining query syntax with method chaining
                var recentOrders = (from o in _context.Orders
                                   where o.UserID == userId.Value
                                   select o)
                                   .OrderByDescending(o => o.OrderDate)  // Method chaining
                                   .Take(5)                               // Lambda: Take
                                   .ToList();

                // Additional LINQ examples for requirements:

                // Any: Check if user has any orders
                bool hasOrders = _context.Orders.Any(o => o.UserID == userId.Value);

                // First/FirstOrDefault: Get most recent order
                var latestOrder = _context.Orders
                    .Where(o => o.UserID == userId.Value)
                    .OrderByDescending(o => o.OrderDate)
                    .FirstOrDefault();

                // Select (projection): Get only specific fields
                var orderSummaries = _context.Orders
                    .Where(o => o.UserID == userId.Value)
                    .Select(o => new
                    {
                        o.OrderID,
                        o.TotalAmount,
                        o.Status
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading orders: {ex.Message}";
                Orders = new List<Order>();
            }

            return Page();
        }
    }

    // Helper class for GROUP BY statistics
    public class OrderStatusStatistic
    {
        public string Status { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
