using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using MaysaraRazorPages.Data;
using System.Data;

namespace MaysaraRazorPages.Pages.Restaurants
{
    /// <summary>
    /// Restaurants Index Page - Uses ADO.NET for direct database access
    /// Demonstrates: SELECT with WHERE, ORDER BY, parameterized queries
    /// </summary>
    public class IndexModel : PageModel
    {
        private readonly AdoNetDataAccess _adoNet;

        public IndexModel(AdoNetDataAccess adoNet)
        {
            _adoNet = adoNet;
        }

        public DataTable? Restaurants { get; set; }
        public string? SearchTerm { get; set; }
        public string? ErrorMessage { get; set; }

        public void OnGet(string? searchTerm)
        {
            SearchTerm = searchTerm;

            try
            {
                string query;
                SqlParameter[]? parameters = null;

                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    // ADO.NET SELECT with WHERE and ORDER BY
                    query = @"SELECT RestaurantID, Name, Address, Phone, Rating, DeliveryFee, IsActive
                              FROM RESTAURANTS
                              WHERE IsActive = 1
                              ORDER BY Rating DESC, Name ASC";
                }
                else
                {
                    // ADO.NET SELECT with parameterized search (SQL injection prevention)
                    query = @"SELECT RestaurantID, Name, Address, Phone, Rating, DeliveryFee, IsActive
                              FROM RESTAURANTS
                              WHERE IsActive = 1 AND Name LIKE @SearchTerm
                              ORDER BY Rating DESC, Name ASC";

                    // Parameterized query to prevent SQL injection
                    parameters = new SqlParameter[]
                    {
                        new SqlParameter("@SearchTerm", SqlDbType.NVarChar, 100)
                        {
                            Value = $"%{searchTerm}%"
                        }
                    };
                }

                // Execute query using ADO.NET
                if (parameters != null)
                {
                    Restaurants = _adoNet.ExecuteQuery(query, parameters);
                }
                else
                {
                    Restaurants = _adoNet.ExecuteQuery(query);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading restaurants: {ex.Message}";
                Restaurants = new DataTable();
            }
        }
    }
}
