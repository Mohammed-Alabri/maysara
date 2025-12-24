using MVCMaysara.Models.Enums;

namespace MVCMaysara.Models.DTOs
{
    public class PlaceOrderRequest
    {
        public string DeliveryAddress { get; set; } = string.Empty;
        public PaymentMethod PaymentMethod { get; set; }
    }
}
