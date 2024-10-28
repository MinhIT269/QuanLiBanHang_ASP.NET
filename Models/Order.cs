using System.ComponentModel.DataAnnotations;

namespace Quan_ly_ban_hang.Models
{
    public class Order
    {
        [Key]
        public Guid OrderId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        public string? OrderCode { get; set; }
        public string? UserName { get; set; }
        public DateTime OrderDate { get; set; }

        [Required, Range(0, double.MaxValue)]
        public decimal TotalAmount { get; set; }

        [Required,MaxLength(50)]
        public string? Status { get; set; }
        public ICollection<OrderDetail> ?OrderDetails { get; set; }
        public User? User { get; set; }

        public Payment? Payment { get; set; }
    }
}
