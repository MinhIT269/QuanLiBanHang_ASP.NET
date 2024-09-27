using Microsoft.EntityFrameworkCore;
using Quan_ly_ban_hang.Models;
using Quan_ly_ban_hang.Request;

namespace Quan_ly_ban_hang.Services
{
    public class ProductService : IProductService
    {
        private readonly DataContext _dataContext;
		private readonly IWebHostEnvironment _env;
		public ProductService(DataContext dataContext, IWebHostEnvironment env)
        {
            _dataContext = dataContext;
			_env = env;

		}
        // Hàm chuyển đổi từ Product sang ProductRequest
        private ProductRequest ConvertToProductRequest(Product product)
        {
            return new ProductRequest
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                Image = product.Image,
                BrandId = product.BrandId,
                BrandName = product.Brand?.Name,
                CategoryName = string.Join(", ", product.ProductCategories
                    .Select(pc => pc.Category.CategoryName)),
                CategoryId = product.ProductCategories
                    .Select(pc => pc.CategoryId)
                    .FirstOrDefault(),
                ProductImages = product.ProductImages?.Select(pi => new ProductImageRequest
                {
                    ProductImageId = pi.ProductImageId,
                    ProductId = pi.ProductId,
                    ImageUrl = pi.ImageUrl
                }).ToList()

            };
        }
        // Hàm chuyển đổi một danh sách Product sang danh sách ProductRequest
        private List<ProductRequest> ConvertToProductRequestList(List<Product> products)
        {
            return products.Select(p => ConvertToProductRequest(p)).ToList();
        }
        // Lay tat ca cac san pham tu csdl
        public async Task<List<ProductRequest>> GetAllProductAsync()
        {
            var products = await _dataContext.Products
            .Include(p => p.ProductCategories)
            .ThenInclude(pc => pc.Category)
            .ToListAsync();

            return ConvertToProductRequestList(products);
        }

        private string GetCategoryNames(IEnumerable<ProductCategory> productCategories)
        {
            return string.Join(", ", productCategories
                .Select(pc => pc.Category?.CategoryName ?? "Unknown"));
        }
        // Tim san pham theo product_id
        public async Task<ProductRequest> GetProductByIdAsync(Guid id)
        {
            var product = await _dataContext.Products
                .Include(p => p.ProductCategories)
                .ThenInclude(pc => pc.Category)
                .Include(p => p.ProductImages) // Bao gồm thông tin về hình ảnh sản phẩm
                .FirstOrDefaultAsync(c => c.ProductId == id);

            if (product == null)
            {
                throw new KeyNotFoundException("Product not found.");
            }

            return ConvertToProductRequest(product);
        }
        // Tim kiem san pham theo ten va danh muc
        public async Task<List<ProductRequest>> FindProductsAsync(string temp, Guid id)
        {
            var products = await _dataContext.Products
             .Include(p => p.ProductCategories)
             .ThenInclude(pc => pc.Category)
             .Where(p => p.Name.ToLower().Contains(temp.ToLower()) &&
                         p.ProductCategories.Any(pc => pc.CategoryId == id))
             .ToListAsync();

            if (!products.Any())
            {
                throw new KeyNotFoundException("Product not found.");
            }

            return ConvertToProductRequestList(products);
        }
        // Tim kiem san pham dua tren temp va danh muc id
        public async Task<List<ProductRequest>> FindProductsByNameAsync(string temp)
        {
            var products = await _dataContext.Products
           .Include(p => p.ProductCategories)
           .ThenInclude(pc => pc.Category)
           .Where(p => p.Name.ToLower().Contains(temp.ToLower()))
           .ToListAsync();

            return ConvertToProductRequestList(products);
        }
        // Dem tong so san pham trong csdl
        public async Task<int> CountProductAsync()
        {
            return await _dataContext.Products.CountAsync();
        }
        public async Task<IEnumerable<ProductRequest>> GetProductsAsync(string searchQuery, int page, int limit, string sortCriteria)
        {
            // Tải các thuộc tính liên kết.
            var query = _dataContext.Products.Include(p => p.ProductCategories).ThenInclude(pc => pc.Category).AsQueryable();

            // Nếu có từ khóa tìm kiếm, lọc các sản phẩm
            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(p => p.Name.Contains(searchQuery));
            }
            switch (sortCriteria)
            {
                case "category":
                    // Sắp xếp theo tên của danh mục đầu tiên (nếu có nhiều danh mục, chỉ lấy danh mục đầu tiên)
                    query = query.OrderBy(p => p.ProductCategories.OrderBy(pc => pc.Category.CategoryName).FirstOrDefault().Category.CategoryName);
                    break;
                case "stock":
                    query = query.OrderByDescending(p => p.Stock);
                    break;
                case "name":
                    query = query.OrderBy(p => p.Name);
                    break;
                case "price":
                    query = query.OrderByDescending(p => p.Price);
                    break;
                default:
                    break;
            }
            // Phân trang: bỏ qua các sản phẩm của các trang trước và lấy sản phẩm của trang hiện tại
            var products = await query.Skip((page - 1) * limit).Take(limit).ToListAsync();

            return products.Select(ConvertToProductRequest);
        }
        // Them san pham vao csdl
        public async Task AddProductAsync(ProductRequest p)
        {
            var product = new Product
            {
                ProductId = Guid.NewGuid(),
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Stock = p.Stock,
                Image = p.Image,
                BrandId = p.BrandId,

            };

            if (p.CategoryId.HasValue)
            {
                var category = await _dataContext.Categories
                    .FindAsync(p.CategoryId.Value);

                if (category != null)
                {
                    _dataContext.Products.Add(product);

                    var productCategory = new ProductCategory
                    {
                        ProductId = product.ProductId,
                        CategoryId = category.CategoryId
                    };
                    _dataContext.ProductCategories.Add(productCategory);
                }
            }

            if (p.ProductImages != null && p.ProductImages.Any())
            {
                foreach (var productImageRequest in p.ProductImages)
                {
                    // Đảm bảo productImageRequest không null trước khi truy cập ImageUrl
                    if (productImageRequest != null)
                    {
                        var productImage = new ProductImage
                        {
                            ProductImageId = Guid.NewGuid(),
                            ProductId = product.ProductId, // ID của sản phẩm vừa tạo
                            ImageUrl = productImageRequest.ImageUrl // Đường dẫn ảnh từ request
                        };

                        // Thêm vào bảng ProductImages
                        _dataContext.ProductImages.Add(productImage);
                    }
                }
            }
            // Lưu các thay đổi vào cơ sở dữ liệu
            await _dataContext.SaveChangesAsync();
        }

        // Cap nhat thong tin san pham
        public async Task UpdateProductAsync(ProductRequest model)
        {
            var existingProduct = await _dataContext.Products
                .Include(p => p.ProductCategories)
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.ProductId == model.ProductId);

            if (existingProduct == null)
            {
                throw new KeyNotFoundException("Product not found.");
            }

            // Xử lý hình ảnh chính nếu có và khác với hình ảnh hiện tại
            if (!string.IsNullOrEmpty(model.Image) && model.Image != existingProduct.Image)
            {
                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", Path.GetFileName(existingProduct.Image));
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }

            // Cập nhật thông tin sản phẩm
            existingProduct.Name = model.Name;
            existingProduct.Description = model.Description;
            existingProduct.Price = model.Price;
            existingProduct.Stock = model.Stock;
            existingProduct.Image = model.Image;
            existingProduct.BrandId = model.BrandId;

            // Xóa các thể loại cũ
            var existingCategories = _dataContext.ProductCategories
                .Where(pc => pc.ProductId == model.ProductId)
                .ToList();

            if (existingCategories.Any())
            {
                _dataContext.ProductCategories.RemoveRange(existingCategories);
            }

            // Thêm thể loại mới
            if (model.CategoryId.HasValue)
            {
                var newCategory = new ProductCategory
                {
                    ProductId = model.ProductId,
                    CategoryId = model.CategoryId.Value
                };

                _dataContext.ProductCategories.Add(newCategory);
            }

            // Xóa các hình ảnh phụ cũ không còn tồn tại trong danh sách mới
            var existingProductImages = _dataContext.ProductImages
                .Where(pi => pi.ProductId == model.ProductId)
                .ToList();

            var newImageUrls = model.ProductImages?.Select(img => img.ImageUrl).ToHashSet() ?? new HashSet<string>();

            if (existingProductImages.Any())
            {
                foreach (var image in existingProductImages.ToList())
                {
                    if (!newImageUrls.Contains(image.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", Path.GetFileName(image.ImageUrl));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);  // Xóa file cũ
                        }
                        _dataContext.ProductImages.Remove(image);
                    }
                }
            }

            // Thêm các hình ảnh phụ mới
            var newProductImages = model.ProductImages?.Where(newImage =>
                !existingProductImages.Any(existingImage => existingImage.ImageUrl == newImage.ImageUrl)).ToList();

            if (newProductImages != null && newProductImages.Any())
            {
                foreach (var newImage in newProductImages)
                {
                    var productImage = new ProductImage
                    {
                        ProductImageId = newImage.ProductImageId,
                        ProductId = newImage.ProductId,
                        ImageUrl = newImage.ImageUrl
                    };

                    _dataContext.ProductImages.Add(productImage);
                }
            }

            // Lưu tất cả các thay đổi vào cơ sở dữ liệu
            await _dataContext.SaveChangesAsync();
        }

        public async Task<bool> DeleteProductAsync(Guid id)
        {
            try
            {
                var product = await _dataContext.Products.FindAsync(id);
                if (product != null)
                {
                    _dataContext.Products.Remove(product);
                    await _dataContext.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting product with ID {id}: {ex.Message}");
            }

        }
        // Tim kiem san pham theo ten
        public async Task<List<ProductRequest>> FindProductsByNameAsync(string name, int page, int limit)
        {
            var query = _dataContext.Products.Include(p => p.ProductCategories)
                .ThenInclude(pc => pc.Category)
                .AsQueryable();
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(p => p.Name.Contains(name));
            }

            var products = await query
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            return products.Select(ConvertToProductRequest).ToList();
        }
        public async Task<List<ProductRequest>> FindProductsByIdAsync(string name, Guid cat, int page, int limit)
        {
            var query = _dataContext.Products
                .Include(p => p.ProductCategories)
                .ThenInclude(pc => pc.Category)
                .Where(p => p.ProductCategories.Any(pc => pc.CategoryId == cat))
                .AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(p => p.Name.Contains(name));
            }

            var products = await query
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            return products.Select(ConvertToProductRequest).ToList();
        }

		public async Task<string> UploadImageAsync(IFormFile image)
		{
			if (image == null || image.Length == 0)
			{
				return null;
			}

			// Tạo đường dẫn lưu trữ tệp trên server
			var filePath = Path.Combine(_env.WebRootPath, "images", image.FileName);

			// Lưu tệp vào server
			using (var stream = new FileStream(filePath, FileMode.Create))
			{
				await image.CopyToAsync(stream);
			}

			// Trả về đường dẫn URL
			return $"/images/{image.FileName}";
		}
	}
}
