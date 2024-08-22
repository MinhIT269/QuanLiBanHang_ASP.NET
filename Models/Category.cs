using System.ComponentModel.DataAnnotations;

namespace Quan_ly_ban_hang.Models
{
	public class Category
	{
		[Key]
		public Guid CategoryId { get; set; }
		[Required, MaxLength(100)]
		public string CategoryName { get; set; }
		[MaxLength(200)]
		public string Description { get; set; }

		public ICollection<ProductCategory> ?ProductCategories { get; set; }
	}
}
