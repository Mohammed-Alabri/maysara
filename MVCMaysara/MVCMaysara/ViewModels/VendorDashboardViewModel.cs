namespace MVCMaysara.ViewModels
{
    public class VendorDashboardViewModel
    {
        public int TotalRestaurants { get; set; }
        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<RestaurantStatViewModel> RestaurantStats { get; set; } = new();
    }

    public class RestaurantStatViewModel
    {
        public string RestaurantName { get; set; } = string.Empty;
        public int OrderCount { get; set; }
        public decimal Revenue { get; set; }
    }
}
