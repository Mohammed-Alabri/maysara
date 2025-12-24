using System.ComponentModel.DataAnnotations;

namespace MaysaraRazorPages.Models.ViewModels
{
    public class RestaurantViewModel
    {
        public int RestaurantID { get; set; }

        [Required(ErrorMessage = "Restaurant name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required")]
        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Delivery fee is required")]
        [Range(0, 100, ErrorMessage = "Delivery fee must be between 0 and 100 OMR")]
        public decimal DeliveryFee { get; set; }
    }
}
