using System.ComponentModel.DataAnnotations;

namespace Quan_ly_ban_hang.Models
{
    public class ShoppingCart
    {
        [Key]
        public Guid CartId { get; set; }
        [Required]
        public Guid CustomerId { get; set; }

        public  DateTime CreatedDate { get; set; }

        public ICollection<CartItem> CartItems { get; set; }

        public Customer Customer { get; set; }

    }
}
