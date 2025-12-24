using MVCMaysara.Models.Enums;

namespace MVCMaysara.ViewModels
{
    public class OrderHistoryViewModel
    {
        public int OrderID { get; set; }
        public string RestaurantName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime OrderDate { get; set; }
        public int ItemCount { get; set; }
    }
}
