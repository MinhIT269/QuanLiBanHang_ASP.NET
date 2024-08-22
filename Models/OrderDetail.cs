using System.ComponentModel.DataAnnotations;

namespace Quan_ly_ban_hang.Models
{
    public class OrderDetail
    {

        [Key]
        public Guid OrderDetailId { get; set; } // PK

        [Required]
        public Guid OrderId { get; set; } // FK

        [Required]
        public Guid ProductId { get; set; } // FK

        [Required, Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required, Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }

        // Navigation properties
        public Order Order { get; set; }
        public Product Product { get; set; }
    }
}
