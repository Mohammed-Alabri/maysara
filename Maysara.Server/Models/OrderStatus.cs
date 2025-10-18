namespace Maysara.Server.Models
{
    /// <summary>
    /// Enumeration representing the various states of an order in the delivery system
    /// </summary>
    public enum OrderStatus
    {
        Pending = 0,
        Confirmed = 1,
        Preparing = 2,
        OutForDelivery = 3,
        Delivered = 4,
        Cancelled = 5
    }
}
