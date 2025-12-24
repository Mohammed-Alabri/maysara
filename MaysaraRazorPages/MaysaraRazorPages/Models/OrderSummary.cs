namespace MaysaraRazorPages.Models
{
    /// <summary>
    /// Model representing the VW_OrderSummary database view
    /// This view combines data from ORDERS, USERS, and RESTAURANTS tables
    /// </summary>
    public class OrderSummary
    {
        public int OrderID { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string RestaurantName { get; set; } = string.Empty;
    }
}
