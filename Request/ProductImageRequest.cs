
namespace Quan_ly_ban_hang.Request
{
    public class ProductImageRequest
    {
        public Guid ProductImageId { get; set; }
        public Guid ProductId { get; set; }
        public string ImageUrl { get; set; } // Đường dẫn đến ảnh chi tiết

    }
}
