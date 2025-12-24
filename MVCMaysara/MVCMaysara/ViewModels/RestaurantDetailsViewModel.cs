using MVCMaysara.Models;

namespace MVCMaysara.ViewModels
{
    public class RestaurantDetailsViewModel
    {
        public int RestaurantID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public decimal Rating { get; set; }
        public decimal DeliveryFee { get; set; }
        public List<Product> Products { get; set; } = new();
    }
}
