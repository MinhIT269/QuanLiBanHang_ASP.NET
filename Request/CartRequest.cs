using System.ComponentModel.DataAnnotations;

namespace Quan_ly_ban_hang.Request
{
    public class CartRequest
    {
        [Required]
        public Guid ProductId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int Quantity { get; set; } = 1; // Mặc định là 1 nếu không truyền vào
        public int Stock { get; set; }
        public decimal Price { get; set; }
		public string? Name { get; set; }
		public string? Image { get; set; }
	}
}
