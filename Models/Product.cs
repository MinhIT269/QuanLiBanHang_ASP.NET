using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing.Drawing2D;

namespace Quan_ly_ban_hang.Models
{
    public class Product
    {
        [Key]
        public Guid ProductId { get; set; }

        [Required,MaxLength(100)]
        public string Name { get; set; }
        [MaxLength(2500)]
        public string Description { get; set; }

        [Required,Range(0,double.MaxValue)]
        public decimal Price { get; set; }
        [Required, Range(0,int.MaxValue)]
        public int Stock { get; set; }
        [MaxLength(255)]
        public string Image {  get; set; }
		public Guid? BrandId { get; set; } // Khóa ngoại cho Brand
		public Brand Brand { get; set; }
		public ICollection<OrderDetail> OrderDetails { get; set; }
        public ICollection<ProductCategory> ProductCategories { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<CartItem> CartItems { get; set; }
        public ICollection<ProductImage> ProductImages { get; set; } // Ảnh chi tiết
		public ICollection<ProductDiscount> ProductDiscounts { get; set; }
	}
}
