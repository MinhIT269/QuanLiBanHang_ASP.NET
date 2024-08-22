using System.ComponentModel.DataAnnotations;

namespace Quan_ly_ban_hang.Models
{
	public class Discount
	{
		[Key]
		public Guid DiscountId { get; set; }
		[Required]
		public string Code { get; set; } // Mã giảm giá
		[Required,Range(0,100)]
		public double Percentage {  get; set; }

		[Required]
		public DateTime StartDate { get; set; }
		[Required]
		public DateTime EndDate { get; set; }

		// Thông tin về số lượng mã giảm giá
		public int MaxUsage { get; set; } // Số lần tối đa có thể sử dụng
		public int CurrentUsage {  get; set; } // Số lần đã sử dụng

		// Thông tin về số lượng mã giảm giá cụ thể
		public int MaxCodes { get; set; } // Số lượng mã giảm giá có thể tạo ra
		public int CurrentCodes { get; set; } // Số lượng mã giảm giá đã tạo ra
		public Guid ProductId { get; set; }
		public ICollection<ProductDiscount> ProductDiscounts { get; set; } // Mối quan hệ nhiều-nhiều với sản phẩm
	}
}
