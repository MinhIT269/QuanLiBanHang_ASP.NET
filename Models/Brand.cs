using System.ComponentModel.DataAnnotations;

namespace Quan_ly_ban_hang.Models
{
	public class Brand
	{
		[Key]
		public Guid BrandId { get; set; }

		[Required, MaxLength(100)]
		public string Name { get; set; }

		[MaxLength(300)]
		public string Description { get; set; }

		public ICollection<Product> Products { get; set; }
	}
}
