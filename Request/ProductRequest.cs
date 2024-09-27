using System.ComponentModel.DataAnnotations;

namespace Quan_ly_ban_hang.Request
{
    public class ProductRequest
    {
        public Guid ProductId { get; set; }
        [Required, MaxLength(100)]
        public string Name { get; set; }
        [MaxLength(2500)]
        public string Description { get; set; }

        [Required, Range(0, double.MaxValue)]
        public decimal Price { get; set; }
        [Required, Range(0, int.MaxValue)]
        public int Stock { get; set; }
        [MaxLength(255)]
        public string? Image { get; set; }
        public Guid? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public Guid? BrandId { get; set; }
		public string? BrandName { get; set; }
		[Range(0, 100)]
		public double? Percentage { get; set; }
        public List<Guid>? CategoryIds { get; set; }

        // Thêm thuộc tính để lưu trữ chi tiết hình ảnh
        public List<ProductImageRequest>? ProductImages { get; set; }
    }
}
