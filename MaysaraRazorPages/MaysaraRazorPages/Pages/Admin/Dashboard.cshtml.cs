using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using MaysaraRazorPages.Data;
using MaysaraRazorPages.Services;
using System.Data;

namespace MaysaraRazorPages.Pages.Admin
{
    /// <summary>
    /// Admin Dashboard Page - Uses ADO.NET for direct database access
    /// Demonstrates: SELECT with aggregates (COUNT, SUM, AVG), GROUP BY, multiple complex queries
    /// </summary>
    public class DashboardModel : PageModel
    {
        private readonly AdoNetDataAccess _adoNet;

        public DashboardModel(AdoNetDataAccess adoNet)
        {
            _adoNet = adoNet;
        }

        // Summary statistics
        public int TotalUsers { get; set; }
        public int TotalRestaurants { get; set; }
        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageOrderValue { get; set; }
        public decimal PendingRevenue { get; set; }

        // Detailed data
        public DataTable? OrdersByStatus { get; set; }
        public DataTable? UsersByRole { get; set; }
        public DataTable? TopRestaurants { get; set; }
        public DataTable? OrderSummaries { get; set; }

        public string? ErrorMessage { get; set; }
        public string? AccessDeniedMessage { get; set; }

        public IActionResult OnGet()
        {
            var userId = SessionManager.GetUserId(HttpContext);
            var userRole = SessionManager.GetUserRole(HttpContext);

            if (userId == null)
            {
                return RedirectToPage("/Auth/Login");
            }

            // Only admins can access dashboard
            if (userRole != "Admin")
            {
                AccessDeniedMessage = "Only administrators can access the dashboard.";
                return Page();
            }

            try
            {
                LoadDashboardData();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading dashboard data: {ex.Message}";
            }

            return Page();
        }

        private void LoadDashboardData()
        {
            // ===== ADO.NET: SELECT with COUNT aggregate =====
            // Get total counts for each entity

            string countUsersQuery = "SELECT COUNT(*) FROM USERS";
            TotalUsers = Convert.ToInt32(_adoNet.ExecuteScalar(countUsersQuery) ?? 0);

            string countRestaurantsQuery = "SELECT COUNT(*) FROM RESTAURANTS WHERE IsActive = 1";
            TotalRestaurants = Convert.ToInt32(_adoNet.ExecuteScalar(countRestaurantsQuery) ?? 0);

            string countProductsQuery = "SELECT COUNT(*) FROM PRODUCTS WHERE IsAvailable = 1";
            TotalProducts = Convert.ToInt32(_adoNet.ExecuteScalar(countProductsQuery) ?? 0);

            string countOrdersQuery = "SELECT COUNT(*) FROM ORDERS";
            TotalOrders = Convert.ToInt32(_adoNet.ExecuteScalar(countOrdersQuery) ?? 0);

            // ===== ADO.NET: SELECT with SUM and AVG aggregates =====
            // Get revenue statistics

            string totalRevenueQuery = @"SELECT ISNULL(SUM(TotalAmount), 0)
                                        FROM ORDERS
                                        WHERE Status = 'Delivered'";
            TotalRevenue = Convert.ToDecimal(_adoNet.ExecuteScalar(totalRevenueQuery) ?? 0);

            string avgOrderQuery = @"SELECT ISNULL(AVG(TotalAmount), 0)
                                    FROM ORDERS";
            AverageOrderValue = Convert.ToDecimal(_adoNet.ExecuteScalar(avgOrderQuery) ?? 0);

            string pendingRevenueQuery = @"SELECT ISNULL(SUM(TotalAmount), 0)
                                          FROM ORDERS
                                          WHERE Status IN ('Pending', 'Confirmed', 'Preparing', 'OutForDelivery')";
            PendingRevenue = Convert.ToDecimal(_adoNet.ExecuteScalar(pendingRevenueQuery) ?? 0);

            // ===== ADO.NET: SELECT with GROUP BY =====
            // Get orders breakdown by status with COUNT and SUM

            string ordersByStatusQuery = @"SELECT Status,
                                                  COUNT(*) as OrderCount,
                                                  ISNULL(SUM(TotalAmount), 0) as TotalAmount
                                           FROM ORDERS
                                           GROUP BY Status
                                           ORDER BY OrderCount DESC";

            OrdersByStatus = _adoNet.ExecuteQuery(ordersByStatusQuery);

            // ===== ADO.NET: SELECT with GROUP BY on Users =====
            // Get users breakdown by role

            string usersByRoleQuery = @"SELECT Role,
                                              COUNT(*) as UserCount
                                       FROM USERS
                                       GROUP BY Role
                                       ORDER BY UserCount DESC";

            UsersByRole = _adoNet.ExecuteQuery(usersByRoleQuery);

            // ===== ADO.NET: SELECT with JOIN and GROUP BY =====
            // Get top restaurants by order count and revenue

            string topRestaurantsQuery = @"SELECT TOP 5
                                                 r.Name as RestaurantName,
                                                 COUNT(o.OrderID) as OrderCount,
                                                 ISNULL(SUM(o.TotalAmount), 0) as TotalRevenue
                                          FROM RESTAURANTS r
                                          LEFT JOIN ORDERS o ON r.RestaurantID = o.RestaurantID
                                          GROUP BY r.RestaurantID, r.Name
                                          HAVING COUNT(o.OrderID) > 0
                                          ORDER BY OrderCount DESC, TotalRevenue DESC";

            TopRestaurants = _adoNet.ExecuteQuery(topRestaurantsQuery);

            // ===== ADO.NET: Query VW_OrderSummary database view =====
            // Demonstrates integration of database view into the application
            string viewQuery = @"SELECT TOP 10 OrderID, OrderDate, Status, TotalAmount, CustomerName, RestaurantName
                                FROM VW_OrderSummary
                                ORDER BY OrderDate DESC";

            OrderSummaries = _adoNet.ExecuteQuery(viewQuery);
        }
    }
}
