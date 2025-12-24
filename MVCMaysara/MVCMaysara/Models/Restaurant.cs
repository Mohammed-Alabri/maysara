using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCMaysara.Models
{
    [Table("RESTAURANTS")]
    public class Restaurant
    {
        [Key]
        public int RestaurantID { get; set; }

        [Required]
        public int OwnerID { get; set; }

        [Required(ErrorMessage = "Restaurant name is required")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required")]
        [StringLength(200)]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        [Range(0, 5, ErrorMessage = "Rating must be between 0 and 5")]
        public decimal Rating { get; set; } = 0.00m;

        [Required(ErrorMessage = "Delivery fee is required")]
        [Range(0, 100, ErrorMessage = "Delivery fee must be between 0 and 100")]
        public decimal DeliveryFee { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation Properties
        [ForeignKey("OwnerID")]
        public User? Owner { get; set; }

        public ICollection<Product>? Products { get; set; }
    }
}
