using System.Text.Json.Serialization;

namespace MaysaraRazorPages.Models
{
    /// <summary>
    /// Shopping cart item - stored in session as JSON
    /// Minimal design: only stores essential data
    /// </summary>
    public class CartItem
    {
        [JsonPropertyName("productId")]
        public int ProductID { get; set; }

        [JsonPropertyName("productName")]
        public string ProductName { get; set; } = string.Empty;

        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("restaurantId")]
        public int RestaurantID { get; set; }

        [JsonPropertyName("restaurantName")]
        public string RestaurantName { get; set; } = string.Empty;

        [JsonIgnore]
        public decimal Subtotal => Price * Quantity;
    }

    /// <summary>
    /// Shopping cart - contains items from ONE restaurant only
    /// </summary>
    public class ShoppingCart
    {
        [JsonPropertyName("restaurantId")]
        public int RestaurantID { get; set; }

        [JsonPropertyName("restaurantName")]
        public string RestaurantName { get; set; } = string.Empty;

        [JsonPropertyName("items")]
        public List<CartItem> Items { get; set; } = new List<CartItem>();

        [JsonIgnore]
        public int TotalItems => Items.Sum(i => i.Quantity);

        [JsonIgnore]
        public decimal TotalAmount => Items.Sum(i => i.Subtotal);
    }
}
