using System.ComponentModel.DataAnnotations;
using MVCMaysara.Models;
using MVCMaysara.Models.Enums;

namespace MVCMaysara.ViewModels
{
    public class CheckoutViewModel
    {
        public ShoppingCart? Cart { get; set; }
        public decimal DeliveryFee { get; set; }
        public decimal GrandTotal => (Cart?.TotalAmount ?? 0) + DeliveryFee;

        [Required(ErrorMessage = "Delivery address is required")]
        [StringLength(300)]
        public string DeliveryAddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "Payment method is required")]
        public PaymentMethod PaymentMethod { get; set; }
    }
}
