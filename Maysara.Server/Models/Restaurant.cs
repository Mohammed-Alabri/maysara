using System.ComponentModel.DataAnnotations;

namespace Maysara.Server.Models
{
    /// <summary>
    /// Represents a restaurant in the Maysara delivery platform
    /// </summary>
    public class Restaurant
    {
        // Properties
        public int RestaurantID { get; set; }

        [Required(ErrorMessage = "Restaurant name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required")]
        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string Phone { get; set; } = string.Empty;

        [Range(0, 5, ErrorMessage = "Rating must be between 0 and 5")]
        public double Rating { get; set; }

        [Range(0, 100, ErrorMessage = "Delivery fee must be between 0 and 100")]
        public decimal DeliveryFee { get; set; }

        public bool IsActive { get; set; }

        public string? ImageUrl { get; set; }

        public string? Cuisine { get; set; }

        // Constructor
        public Restaurant()
        {
            IsActive = true;
            Rating = 0;
            DeliveryFee = 0;
        }

        public Restaurant(int id, string name, string address, string phone, double rating, decimal deliveryFee)
        {
            RestaurantID = id;
            Name = name;
            Address = address;
            Phone = phone;
            Rating = rating;
            DeliveryFee = deliveryFee;
            IsActive = true;
        }

        // Methods
        public string GetDisplayInfo()
        {
            return $"{Name} - {Cuisine ?? "Various"} | Rating: {Rating:F1} | Delivery: {DeliveryFee:C}";
        }

        public bool IsAvailable()
        {
            return IsActive;
        }

        public void ToggleActive()
        {
            IsActive = !IsActive;
        }
    }
}
