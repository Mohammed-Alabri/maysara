using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using MaysaraRazorPages.Data;
using MaysaraRazorPages.Models.ViewModels;
using MaysaraRazorPages.Services;
using System.Data;

namespace MaysaraRazorPages.Pages.Restaurants
{
    /// <summary>
    /// Restaurant Management Page - Uses ADO.NET for all CRUD operations
    /// Demonstrates: INSERT, UPDATE, DELETE (soft delete) with parameterized queries
    /// </summary>
    public class ManageModel : PageModel
    {
        private readonly AdoNetDataAccess _adoNet;

        public ManageModel(AdoNetDataAccess adoNet)
        {
            _adoNet = adoNet;
        }

        [BindProperty]
        public RestaurantViewModel Input { get; set; } = new RestaurantViewModel();

        public DataTable? VendorRestaurants { get; set; }
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }
        public bool IsEditMode { get; set; }

        public IActionResult OnGet(int? editId)
        {
            var userId = SessionManager.GetUserId(HttpContext);
            var userRole = SessionManager.GetUserRole(HttpContext);

            if (userId == null)
            {
                return RedirectToPage("/Auth/Login");
            }

            // Only vendors can manage restaurants
            if (userRole != "Vendor")
            {
                ErrorMessage = "Only vendors can manage restaurants.";
                return Page();
            }

            LoadVendorRestaurants(userId.Value);

            // If editing, load restaurant details
            if (editId.HasValue)
            {
                LoadRestaurantForEdit(editId.Value, userId.Value);
            }

            return Page();
        }

        /// <summary>
        /// ADO.NET INSERT - Add new restaurant
        /// </summary>
        public IActionResult OnPostAdd()
        {
            var userId = SessionManager.GetUserId(HttpContext);
            var userRole = SessionManager.GetUserRole(HttpContext);

            if (userId == null)
            {
                return RedirectToPage("/Auth/Login");
            }

            if (userRole != "Vendor")
            {
                ErrorMessage = "Only vendors can add restaurants.";
                return Page();
            }

            if (!ModelState.IsValid)
            {
                LoadVendorRestaurants(userId.Value);
                return Page();
            }

            try
            {
                // ADO.NET INSERT operation with parameterized query
                string insertQuery = @"INSERT INTO RESTAURANTS
                                      (OwnerID, Name, Address, Phone, DeliveryFee,
                                       Rating, IsActive, CreatedAt)
                                      VALUES (@OwnerID, @Name, @Address, @Phone,
                                             @DeliveryFee, 0.00, 1, GETDATE())";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@OwnerID", SqlDbType.Int) { Value = userId.Value },
                    new SqlParameter("@Name", SqlDbType.NVarChar, 100) { Value = Input.Name },
                    new SqlParameter("@Address", SqlDbType.NVarChar, 200) { Value = Input.Address },
                    new SqlParameter("@Phone", SqlDbType.NVarChar, 20) { Value = Input.Phone },
                    new SqlParameter("@DeliveryFee", SqlDbType.Decimal) { Value = Input.DeliveryFee }
                };

                int rowsAffected = _adoNet.ExecuteNonQuery(insertQuery, parameters);

                if (rowsAffected > 0)
                {
                    SuccessMessage = "Restaurant added successfully!";
                    // Clear form
                    ModelState.Clear();
                    Input = new RestaurantViewModel();
                }
                else
                {
                    ErrorMessage = "Failed to add restaurant. Please try again.";
                }
            }
            catch (SqlException ex)
            {
                ErrorMessage = $"Database error: {ex.Message}";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error adding restaurant: {ex.Message}";
            }

            LoadVendorRestaurants(userId.Value);
            return Page();
        }

        /// <summary>
        /// ADO.NET UPDATE - Edit existing restaurant
        /// </summary>
        public IActionResult OnPostUpdate()
        {
            var userId = SessionManager.GetUserId(HttpContext);
            var userRole = SessionManager.GetUserRole(HttpContext);

            if (userId == null)
            {
                return RedirectToPage("/Auth/Login");
            }

            if (userRole != "Vendor")
            {
                ErrorMessage = "Only vendors can update restaurants.";
                return Page();
            }

            if (!ModelState.IsValid)
            {
                IsEditMode = true;
                LoadVendorRestaurants(userId.Value);
                return Page();
            }

            try
            {
                // ADO.NET UPDATE operation with parameterized query
                string updateQuery = @"UPDATE RESTAURANTS
                                      SET Name = @Name,
                                          Address = @Address,
                                          Phone = @Phone,
                                          DeliveryFee = @DeliveryFee
                                      WHERE RestaurantID = @RestaurantID
                                      AND OwnerID = @OwnerID";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@RestaurantID", SqlDbType.Int) { Value = Input.RestaurantID },
                    new SqlParameter("@OwnerID", SqlDbType.Int) { Value = userId.Value },
                    new SqlParameter("@Name", SqlDbType.NVarChar, 100) { Value = Input.Name },
                    new SqlParameter("@Address", SqlDbType.NVarChar, 200) { Value = Input.Address },
                    new SqlParameter("@Phone", SqlDbType.NVarChar, 20) { Value = Input.Phone },
                    new SqlParameter("@DeliveryFee", SqlDbType.Decimal) { Value = Input.DeliveryFee }
                };

                int rowsAffected = _adoNet.ExecuteNonQuery(updateQuery, parameters);

                if (rowsAffected > 0)
                {
                    SuccessMessage = "Restaurant updated successfully!";
                    // Clear form
                    ModelState.Clear();
                    Input = new RestaurantViewModel();
                    IsEditMode = false;
                }
                else
                {
                    ErrorMessage = "Restaurant not found or access denied.";
                }
            }
            catch (SqlException ex)
            {
                ErrorMessage = $"Database error: {ex.Message}";
                IsEditMode = true;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error updating restaurant: {ex.Message}";
                IsEditMode = true;
            }

            LoadVendorRestaurants(userId.Value);
            return Page();
        }

        /// <summary>
        /// ADO.NET DELETE - Soft delete (set IsActive = 0)
        /// </summary>
        public IActionResult OnPostDeactivate(int restaurantId)
        {
            var userId = SessionManager.GetUserId(HttpContext);
            var userRole = SessionManager.GetUserRole(HttpContext);

            if (userId == null)
            {
                return RedirectToPage("/Auth/Login");
            }

            if (userRole != "Vendor")
            {
                ErrorMessage = "Only vendors can deactivate restaurants.";
                return Page();
            }

            try
            {
                // ADO.NET UPDATE for soft delete (set IsActive = 0)
                string deleteQuery = @"UPDATE RESTAURANTS
                                      SET IsActive = 0
                                      WHERE RestaurantID = @RestaurantID
                                      AND OwnerID = @OwnerID";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@RestaurantID", SqlDbType.Int) { Value = restaurantId },
                    new SqlParameter("@OwnerID", SqlDbType.Int) { Value = userId.Value }
                };

                int rowsAffected = _adoNet.ExecuteNonQuery(deleteQuery, parameters);

                if (rowsAffected > 0)
                {
                    SuccessMessage = "Restaurant deactivated successfully!";
                }
                else
                {
                    ErrorMessage = "Restaurant not found or access denied.";
                }
            }
            catch (SqlException ex)
            {
                ErrorMessage = $"Database error: {ex.Message}";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error deactivating restaurant: {ex.Message}";
            }

            LoadVendorRestaurants(userId.Value);
            return Page();
        }

        /// <summary>
        /// ADO.NET SELECT - Load vendor's restaurants
        /// </summary>
        private void LoadVendorRestaurants(int userId)
        {
            try
            {
                string query = @"SELECT RestaurantID, Name, Address, Phone,
                                DeliveryFee, IsActive, Rating, CreatedAt
                                FROM RESTAURANTS
                                WHERE OwnerID = @OwnerID
                                ORDER BY Name";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@OwnerID", SqlDbType.Int) { Value = userId }
                };

                VendorRestaurants = _adoNet.ExecuteQuery(query, parameters);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading restaurants: {ex.Message}";
                VendorRestaurants = new DataTable();
            }
        }

        /// <summary>
        /// ADO.NET SELECT - Load restaurant for editing
        /// </summary>
        private void LoadRestaurantForEdit(int restaurantId, int userId)
        {
            try
            {
                string query = @"SELECT RestaurantID, Name, Address, Phone, DeliveryFee
                                FROM RESTAURANTS
                                WHERE RestaurantID = @RestaurantID
                                AND OwnerID = @OwnerID";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@RestaurantID", SqlDbType.Int) { Value = restaurantId },
                    new SqlParameter("@OwnerID", SqlDbType.Int) { Value = userId }
                };

                DataTable result = _adoNet.ExecuteQuery(query, parameters);

                if (result.Rows.Count > 0)
                {
                    var row = result.Rows[0];
                    Input = new RestaurantViewModel
                    {
                        RestaurantID = Convert.ToInt32(row["RestaurantID"]),
                        Name = row["Name"].ToString() ?? string.Empty,
                        Address = row["Address"].ToString() ?? string.Empty,
                        Phone = row["Phone"].ToString() ?? string.Empty,
                        DeliveryFee = Convert.ToDecimal(row["DeliveryFee"])
                    };
                    IsEditMode = true;
                }
                else
                {
                    ErrorMessage = "Restaurant not found or access denied.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading restaurant: {ex.Message}";
            }
        }
    }
}
