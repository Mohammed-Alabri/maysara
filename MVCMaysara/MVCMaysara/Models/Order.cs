using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MVCMaysara.Models.Enums;

namespace MVCMaysara.Models
{
    [Table("ORDERS")]
    public class Order
    {
        [Key]
        public int OrderID { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required]
        public int RestaurantID { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Total amount must be greater than 0")]
        public decimal TotalAmount { get; set; }


        public string DeliveryAddress { get; set; } = string.Empty;

        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        [Required]
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public DateTime OrderDate { get; set; } = DateTime.Now;

        // Navigation Properties
        [ForeignKey("UserID")]
        public User? User { get; set; }

        [ForeignKey("RestaurantID")]
        public Restaurant? Restaurant { get; set; }

        public ICollection<OrderItem>? OrderItems { get; set; }
    }
}
