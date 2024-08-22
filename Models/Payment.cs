using System.ComponentModel.DataAnnotations;

namespace Quan_ly_ban_hang.Models
{
    public class Payment
    {
        [Key]
        public Guid PaymentId { get; set; } // PK

        [Required]
        public Guid OrderId { get; set; } // FK

        public DateTime PaymentDate { get; set; }

        [Required, Range(0, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required, MaxLength(50)]
        public string PaymentMethod { get; set; } // (e.g., Credit Card, Bank Transfer)

        // Navigation properties
        public Order Order { get; set; }
    }
}
