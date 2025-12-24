using MVCMaysara.Models;

namespace MVCMaysara.Services
{
    /// <summary>
    /// Session Manager for handling user authentication state
    /// Stores and retrieves user information across HTTP requests
    /// </summary>
    public static class SessionManager
    {
        // Session keys
        private const string UserIdKey = "UserId";
        private const string UserNameKey = "UserName";
        private const string UserEmailKey = "UserEmail";
        private const string UserRoleKey = "UserRole";

        /// <summary>
        /// Sets user session after successful login
        /// </summary>
        public static void SetUserSession(HttpContext context, User user)
        {
            context.Session.SetInt32(UserIdKey, user.UserID);
            context.Session.SetString(UserNameKey, user.Name);
            context.Session.SetString(UserEmailKey, user.Email);
            context.Session.SetString(UserRoleKey, user.Role.ToString());
        }

        /// <summary>
        /// Gets the logged-in user's ID
        /// </summary>
        public static int? GetUserId(HttpContext context)
        {
            return context.Session.GetInt32(UserIdKey);
        }

        /// <summary>
        /// Gets the logged-in user's name
        /// </summary>
        public static string? GetUserName(HttpContext context)
        {
            return context.Session.GetString(UserNameKey);
        }

        /// <summary>
        /// Gets the logged-in user's email
        /// </summary>
        public static string? GetUserEmail(HttpContext context)
        {
            return context.Session.GetString(UserEmailKey);
        }

        /// <summary>
        /// Gets the logged-in user's role
        /// </summary>
        public static string? GetUserRole(HttpContext context)
        {
            return context.Session.GetString(UserRoleKey);
        }

        /// <summary>
        /// Checks if user is authenticated
        /// </summary>
        public static bool IsAuthenticated(HttpContext context)
        {
            return GetUserId(context).HasValue;
        }

        /// <summary>
        /// Clears user session (logout)
        /// </summary>
        public static void ClearSession(HttpContext context)
        {
            context.Session.Clear();
        }

        // ========== Shopping Cart Methods ==========

        // Session key for shopping cart
        private const string CartKey = "ShoppingCart";

        /// <summary>
        /// Gets the current shopping cart from session
        /// </summary>
        public static ShoppingCart? GetCart(HttpContext context)
        {
            var cartJson = context.Session.GetString(CartKey);
            if (string.IsNullOrEmpty(cartJson))
                return null;

            return System.Text.Json.JsonSerializer.Deserialize<ShoppingCart>(cartJson);
        }

        /// <summary>
        /// Saves the shopping cart to session
        /// </summary>
        private static void SaveCart(HttpContext context, ShoppingCart cart)
        {
            var cartJson = System.Text.Json.JsonSerializer.Serialize(cart);
            context.Session.SetString(CartKey, cartJson);
        }

        /// <summary>
        /// Adds item to cart or updates quantity if exists
        /// Enforces single-restaurant rule
        /// </summary>
        public static bool AddToCart(HttpContext context, int productId, string productName,
            decimal price, int quantity, int restaurantId, string restaurantName)
        {
            var cart = GetCart(context);

            // Create new cart if empty
            if (cart == null)
            {
                cart = new ShoppingCart
                {
                    RestaurantID = restaurantId,
                    RestaurantName = restaurantName
                };
            }

            // Check restaurant mismatch - cart can only contain items from ONE restaurant
            if (cart.RestaurantID != restaurantId)
            {
                return false; // Cannot add items from different restaurant
            }

            // Update existing item or add new
            var existingItem = cart.Items.FirstOrDefault(i => i.ProductID == productId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    ProductID = productId,
                    ProductName = productName,
                    Price = price,
                    Quantity = quantity,
                    RestaurantID = restaurantId,
                    RestaurantName = restaurantName
                });
            }

            SaveCart(context, cart);
            return true;
        }

        /// <summary>
        /// Updates item quantity in cart (or removes if quantity = 0)
        /// </summary>
        public static void UpdateCartItem(HttpContext context, int productId, int quantity)
        {
            var cart = GetCart(context);
            if (cart == null) return;

            var item = cart.Items.FirstOrDefault(i => i.ProductID == productId);
            if (item != null)
            {
                if (quantity <= 0)
                {
                    cart.Items.Remove(item);
                }
                else
                {
                    item.Quantity = quantity;
                }

                // Clear cart if empty
                if (cart.Items.Count == 0)
                {
                    ClearCart(context);
                }
                else
                {
                    SaveCart(context, cart);
                }
            }
        }

        /// <summary>
        /// Clears the shopping cart
        /// </summary>
        public static void ClearCart(HttpContext context)
        {
            context.Session.Remove(CartKey);
        }

        /// <summary>
        /// Gets cart item count for badge display
        /// </summary>
        public static int GetCartItemCount(HttpContext context)
        {
            var cart = GetCart(context);
            return cart?.TotalItems ?? 0;
        }
    }
}
