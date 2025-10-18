using System.ComponentModel.DataAnnotations;

namespace Maysara.Server.Models
{
    /// <summary>
    /// Represents a product/menu item from a restaurant
    /// </summary>
    public class Product
    {
        // Properties
        public int ProductID { get; set; }

        public int RestaurantID { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 10000, ErrorMessage = "Price must be between 0.01 and 10000")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public string Category { get; set; } = string.Empty;

        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative")]
        public int Stock { get; set; }

        public bool IsAvailable { get; set; }

        public string? ImageUrl { get; set; }

        // Constructor
        public Product()
        {
            IsAvailable = true;
            Stock = 0;
        }

        public Product(int id, int restaurantId, string name, string description, decimal price, string category, int stock)
        {
            ProductID = id;
            RestaurantID = restaurantId;
            Name = name;
            Description = description;
            Price = price;
            Category = category;
            Stock = stock;
            IsAvailable = stock > 0;
        }

        // Methods
        public string GetPriceDisplay()
        {
            return $"{Price:C}";
        }

        public bool IsInStock()
        {
            return Stock > 0 && IsAvailable;
        }

        public void UpdateStock(int quantity)
        {
            Stock += quantity;
            IsAvailable = Stock > 0;
        }

        public bool CanOrder(int quantity)
        {
            return IsAvailable && Stock >= quantity;
        }

        public string GetStockStatus()
        {
            if (!IsAvailable) return "Unavailable";
            if (Stock == 0) return "Out of Stock";
            if (Stock < 5) return "Low Stock";
            return "In Stock";
        }
    }
}
