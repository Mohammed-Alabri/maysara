using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MVCMaysara.Models.Enums;

namespace MVCMaysara.Models
{
    [Table("USERS")]
    public class User
    {
        [Key]
        public int UserID { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(255)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        [StringLength(300)]
        public string? Address { get; set; }

        [Required]
        public UserRole Role { get; set; } = UserRole.Customer;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
