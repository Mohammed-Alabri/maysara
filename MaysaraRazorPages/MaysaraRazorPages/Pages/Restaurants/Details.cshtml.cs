using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using MaysaraRazorPages.Data;
using MaysaraRazorPages.Services;
using System.Data;

namespace MaysaraRazorPages.Pages.Restaurants
{
    /// <summary>
    /// Restaurant Details Page - Uses ADO.NET for direct database access
    /// Demonstrates: SELECT with JOIN, parameterized queries, multiple queries on same page
    /// </summary>
    public class DetailsModel : PageModel
    {
        private readonly AdoNetDataAccess _adoNet;

        public DetailsModel(AdoNetDataAccess adoNet)
        {
            _adoNet = adoNet;
        }

        public DataTable? Restaurant { get; set; }
        public DataTable? Products { get; set; }
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage => TempData["SuccessMessage"]?.ToString();

        public IActionResult OnGet(int? id)
        {
            if (id == null)
            {
                ErrorMessage = "Invalid restaurant ID.";
                return Page();
            }

            try
            {
                // ADO.NET SELECT with parameterized query - Get restaurant details
                string restaurantQuery = @"SELECT RestaurantID, OwnerID, Name, Address, Phone,
                                          Rating, DeliveryFee, IsActive
                                          FROM RESTAURANTS
                                          WHERE RestaurantID = @RestaurantID";

                SqlParameter[] restaurantParams = new SqlParameter[]
                {
                    new SqlParameter("@RestaurantID", SqlDbType.Int) { Value = id.Value }
                };

                Restaurant = _adoNet.ExecuteQuery(restaurantQuery, restaurantParams);

                if (Restaurant == null || Restaurant.Rows.Count == 0)
                {
                    ErrorMessage = "Restaurant not found.";
                    return Page();
                }

                // ADO.NET SELECT with JOIN - Get products for this restaurant
                string productsQuery = @"SELECT p.ProductID, p.Name, p.Description, p.Price,
                                        p.Category, p.Stock, p.IsAvailable,
                                        r.Name as RestaurantName
                                        FROM PRODUCTS p
                                        INNER JOIN RESTAURANTS r ON p.RestaurantID = r.RestaurantID
                                        WHERE p.RestaurantID = @RestaurantID
                                        ORDER BY p.Category, p.Name";

                SqlParameter[] productsParams = new SqlParameter[]
                {
                    new SqlParameter("@RestaurantID", SqlDbType.Int) { Value = id.Value }
                };

                Products = _adoNet.ExecuteQuery(productsQuery, productsParams);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading restaurant details: {ex.Message}";
            }

            return Page();
        }

        public IActionResult OnPost(int productId, int quantity)
        {
            var userId = SessionManager.GetUserId(HttpContext);
            if (userId == null)
            {
                return RedirectToPage("/Auth/Login");
            }

            // Reload page data for display
            var restaurantId = int.Parse(Request.Query["id"]!);
            OnGet(restaurantId);

            try
            {
                // Fetch product details from database
                string query = @"SELECT p.ProductID, p.Name, p.Price, p.RestaurantID,
                                p.IsAvailable, p.Stock, r.Name as RestaurantName
                                FROM PRODUCTS p
                                INNER JOIN RESTAURANTS r ON p.RestaurantID = r.RestaurantID
                                WHERE p.ProductID = @ProductID";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@ProductID", SqlDbType.Int) { Value = productId }
                };

                var productData = _adoNet.ExecuteQuery(query, parameters);

                if (productData == null || productData.Rows.Count == 0)
                {
                    ErrorMessage = "Product not found.";
                    return Page();
                }

                var row = productData.Rows[0];
                bool isAvailable = Convert.ToBoolean(row["IsAvailable"]);
                int stock = Convert.ToInt32(row["Stock"]);

                if (!isAvailable || stock < quantity)
                {
                    ErrorMessage = "Product is not available or insufficient stock.";
                    return Page();
                }

                // Add to cart using SessionManager
                bool success = SessionManager.AddToCart(
                    HttpContext,
                    productId,
                    row["Name"].ToString()!,
                    Convert.ToDecimal(row["Price"]),
                    quantity,
                    Convert.ToInt32(row["RestaurantID"]),
                    row["RestaurantName"].ToString()!
                );

                if (!success)
                {
                    ErrorMessage = "Cannot add items from different restaurants. Please clear your cart first.";
                    return Page();
                }

                TempData["SuccessMessage"] = $"Added {quantity} x {row["Name"]} to cart!";
                return RedirectToPage(new { id = restaurantId });
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error adding to cart: {ex.Message}";
                return Page();
            }
        }
    }
}
