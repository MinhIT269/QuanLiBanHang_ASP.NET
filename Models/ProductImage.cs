using System.ComponentModel.DataAnnotations;

namespace Quan_ly_ban_hang.Models
{
    public class ProductImage
    {
        [Key]
        public Guid ProductImageId { get; set; }
        public Guid ProductId { get; set; }
        public string? ImageUrl { get; set; } // Đường dẫn đến ảnh chi tiết

        public Product? Product { get; set; }
    }
}
