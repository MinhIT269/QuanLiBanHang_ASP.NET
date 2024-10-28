namespace Quan_ly_ban_hang.Models
{
	public class ProductDiscount
	{
		public Guid ProductId { get; set; }
		public Product Product { get; set; }

		public Guid DiscountId { get; set; }
		public Discount? Discount { get; set; }
	}
}
