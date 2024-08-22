using Microsoft.EntityFrameworkCore;
using Quan_ly_ban_hang.Models;
using Quan_ly_ban_hang.Request;

namespace Quan_ly_ban_hang.Services
{
    public class ProductService : IProductService
    {
        private readonly DataContext _dataContext;
        public ProductService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<List<ProductRequest>> GetAllProductAsync()
        {
            return await _dataContext.Products.Select(p => new ProductRequest
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Stock = p.Stock,
                Image = p.Image,
                CategoryName = string.Join(", ", p.ProductCategories
                .Select(pc => pc.Category.CategoryName)),
            }).ToListAsync();
        }
        private string GetCategoryNames(IEnumerable<ProductCategory> productCategories)
        {
            return string.Join(", ", productCategories
                .Select(pc => pc.Category?.CategoryName ?? "Unknown"));
        }
        public async Task<ProductRequest> GetProductByIdAsync(Guid id)
        {
            // Tim san pham theo id
            // Tìm sản phẩm theo id
            var product = await _dataContext.Products
                .Include(p => p.ProductCategories)
                    .ThenInclude(pc => pc.Category)
                .Include(p => p.Brand) // Bao gồm thương hiệu
                .FirstOrDefaultAsync(c => c.ProductId == id);
            var brandName = await _dataContext.Brands
                .Where(b => b.BrandId == product.BrandId)
                .Select(b => b.Name)
                .FirstOrDefaultAsync();
            if (product == null)
            {
                throw new KeyNotFoundException("Product not found.");
            }
            var productRequest = new ProductRequest
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                Image = product.Image,
                BrandName = brandName,
                CategoryName = GetCategoryNames(product.ProductCategories),
                BrandId = product.BrandId,
                CategoryId = product.ProductCategories
                        .Select(pc => pc.CategoryId)
                        .FirstOrDefault()
            };
            return productRequest;
        }
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

            var productRequests = products.Select(p => new ProductRequest
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Stock = p.Stock,
                Image = p.Image,
                // Lấy tên danh mục từ ProductCategories
                CategoryName = string.Join(", ", p.ProductCategories
                    .Where(pc => pc.CategoryId == id)
                    .Select(pc => pc.Category.CategoryName)),
            }).ToList();

            return productRequests;
        }

        public async Task<List<ProductRequest>> FindProductsByNameAsync(String temp)
        {
            var product = await _dataContext.Products
            .Include(p => p.ProductCategories)  // Bao gồm ProductCategories
            .ThenInclude(pc => pc.Category)   // Bao gồm Category
            .Where(p => p.Name.ToLower().Contains(temp.ToLower()))
            .ToListAsync();
            //var product = await _dataContext.Products.Where(p => p.Name.ToLower().Contains(temp.ToLower())).ToListAsync();
            if (product == null)
            {
                throw new KeyNotFoundException("Product not found.");
            }
            var productRequests = product.Select(p => new ProductRequest
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Stock = p.Stock,
                Image = p.Image,
                // Lấy tên danh mục từ ProductCategories
                CategoryName = string.Join(", ", p.ProductCategories.Select(pc => pc.Category.CategoryName)),
            }).ToList();
            return productRequests;
        }
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
            var productRequests = products.Select(p => new ProductRequest
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Stock = p.Stock,
                Image = p.Image,
                // Lấy tên danh mục từ ProductCategories
                CategoryName = string.Join(", ", p.ProductCategories.Select(pc => pc.Category.CategoryName)),
            }).ToList();

            return productRequests;
        }
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

            await _dataContext.SaveChangesAsync();
        }
        public async Task UpdateProductAsync(ProductRequest model)
        {
            var existingProduct = await _dataContext.Products
                .Include(p => p.ProductCategories)
                .FirstOrDefaultAsync(p => p.ProductId == model.ProductId);

            if (existingProduct == null)
            {
                throw new KeyNotFoundException("Product not found.");
            }

            // Xử lý hình ảnh mới nếu có
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

            // Lưu thay đổi vào cơ sở dữ liệu
            await _dataContext.SaveChangesAsync();

            // Lưu các thay đổi vào cơ sở dữ liệu
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
    }
}
