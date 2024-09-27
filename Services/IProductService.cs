using Quan_ly_ban_hang.Request;

namespace Quan_ly_ban_hang.Services
{
	public interface IProductService
	{
		// IEnumerable tạo ra một đối tượng có thể lặp qua tất cả các phần tử. Không thể thay đổi nội dung của tập hợp
		Task<IEnumerable<ProductRequest>> GetProductsAsync(string searchQuery, int page, int limit, string sortCriteria); 
		// List trả về danh sách phần tử có thể thay đổi cho phép thêm, xóa phần tử của danh sách
		Task<List<ProductRequest>> GetAllProductAsync();
		Task<ProductRequest> GetProductByIdAsync(Guid id);
		Task<List<ProductRequest>> FindProductsAsync(String temp, Guid id);
		Task<List<ProductRequest>> FindProductsByNameAsync(String temp);
		Task<int> CountProductAsync();
		Task AddProductAsync(ProductRequest product);
		Task UpdateProductAsync(ProductRequest model);
		Task<bool> DeleteProductAsync(Guid id);
		Task<List<ProductRequest>> FindProductsByNameAsync(string name, int page, int limit);
		Task<List<ProductRequest>> FindProductsByIdAsync(string name, Guid cat, int page, int limit);
		Task<string> UploadImageAsync(IFormFile image);

	}
}
