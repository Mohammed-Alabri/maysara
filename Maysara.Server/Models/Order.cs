using System.ComponentModel.DataAnnotations;

namespace Maysara.Server.Models
{
    /// <summary>
    /// Represents a customer order in the delivery system
    /// </summary>
    public class Order
    {
        // Properties
        public int OrderID { get; set; }

        [Required(ErrorMessage = "User ID is required")]
        public string UserID { get; set; } = string.Empty;

        [Required(ErrorMessage = "Restaurant ID is required")]
        public int RestaurantID { get; set; }

        [Required(ErrorMessage = "Total amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Total amount must be greater than 0")]
        public decimal TotalAmount { get; set; }

        [Required(ErrorMessage = "Delivery address is required")]
        [StringLength(300, MinimumLength = 5, ErrorMessage = "Delivery address must be between 5 and 300 characters")]
        public string DeliveryAddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "Payment method is required")]
        public string PaymentMethod { get; set; } = string.Empty;

        public OrderStatus Status { get; set; }

        public DateTime OrderDate { get; set; }

        public DateTime? DeliveryDate { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format")]
        public string? CustomerPhone { get; set; }

        public string? SpecialInstructions { get; set; }

        public List<OrderItem> Items { get; set; } = new List<OrderItem>();

        // Constructor
        public Order()
        {
            Status = OrderStatus.Pending;
            OrderDate = DateTime.Now;
            Items = new List<OrderItem>();
        }

        public Order(int id, string userId, int restaurantId, decimal totalAmount, string deliveryAddress, string paymentMethod)
        {
            OrderID = id;
            UserID = userId;
            RestaurantID = restaurantId;
            TotalAmount = totalAmount;
            DeliveryAddress = deliveryAddress;
            PaymentMethod = paymentMethod;
            Status = OrderStatus.Pending;
            OrderDate = DateTime.Now;
            Items = new List<OrderItem>();
        }

        // Methods
        public void UpdateStatus(OrderStatus newStatus)
        {
            Status = newStatus;
            if (newStatus == OrderStatus.Delivered)
            {
                DeliveryDate = DateTime.Now;
            }
        }

        public string GetStatusDisplay()
        {
            return Status switch
            {
                OrderStatus.Pending => "Pending Confirmation",
                OrderStatus.Confirmed => "Order Confirmed",
                OrderStatus.Preparing => "Being Prepared",
                OrderStatus.OutForDelivery => "Out for Delivery",
                OrderStatus.Delivered => "Delivered",
                OrderStatus.Cancelled => "Cancelled",
                _ => "Unknown Status"
            };
        }

        public bool CanCancel()
        {
            return Status == OrderStatus.Pending || Status == OrderStatus.Confirmed;
        }

        public void Cancel()
        {
            if (CanCancel())
            {
                Status = OrderStatus.Cancelled;
            }
            else
            {
                throw new InvalidOperationException("Order cannot be cancelled at this stage.");
            }
        }

        public void AddItem(OrderItem item)
        {
            Items.Add(item);
            RecalculateTotal();
        }

        public void RemoveItem(int productId)
        {
            Items.RemoveAll(i => i.ProductID == productId);
            RecalculateTotal();
        }

        public void RecalculateTotal()
        {
            TotalAmount = Items.Sum(i => i.Subtotal);
        }

        public TimeSpan GetTimeSinceOrder()
        {
            return DateTime.Now - OrderDate;
        }

        public string GetEstimatedDeliveryTime()
        {
            if (Status == OrderStatus.Delivered)
                return "Delivered";

            var estimatedMinutes = Status switch
            {
                OrderStatus.Pending => 45,
                OrderStatus.Confirmed => 40,
                OrderStatus.Preparing => 30,
                OrderStatus.OutForDelivery => 15,
                _ => 0
            };

            return $"{estimatedMinutes} minutes";
        }
    }

    /// <summary>
    /// Represents an item in an order
    /// </summary>
    public class OrderItem
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal => UnitPrice * Quantity;

        public OrderItem() { }

        public OrderItem(int productId, string productName, decimal unitPrice, int quantity)
        {
            ProductID = productId;
            ProductName = productName;
            UnitPrice = unitPrice;
            Quantity = quantity;
        }
    }
}
