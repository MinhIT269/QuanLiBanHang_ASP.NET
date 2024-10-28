using System.ComponentModel.DataAnnotations;

namespace Quan_ly_ban_hang.Models
{
    public class CartItem
    {
        [Key]
        public Guid CartItemId { get; set; }
        [Required]
        public Guid ProductId { get; set; }

        [Required]
        public Guid CartId { get; set; }

        [Required, Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        public ShoppingCart? Shopping_Cart { get; set; }
        public Product? Product { get; set; }
    }
}
