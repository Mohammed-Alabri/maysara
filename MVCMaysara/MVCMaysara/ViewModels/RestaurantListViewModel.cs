namespace MVCMaysara.ViewModels
{
    public class RestaurantListViewModel
    {
        public int RestaurantID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public decimal Rating { get; set; }
        public decimal DeliveryFee { get; set; }
        public bool IsActive { get; set; }
        public int ProductCount { get; set; }
    }
}
