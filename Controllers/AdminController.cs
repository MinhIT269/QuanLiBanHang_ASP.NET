using Microsoft.AspNetCore.Mvc;
using Quan_ly_ban_hang.Request;
using Quan_ly_ban_hang.Services;

namespace Quan_ly_ban_hang.Controllers
{
    public class AdminController : Controller
    {
        private const int _itemsPerPage = 10;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IBrandService _brandService;

        public AdminController(IProductService productService, ICategoryService categoryService, IBrandService brandService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _brandService = brandService;
        }
        public async Task<IActionResult> Index()
        {
            return View();
        }
        public async Task<IActionResult> List()
        {
            var products = await _productService.GetAllProductAsync();
            return View();
        }

        [Route("admin/product/list")]
        public async Task<IActionResult> ListA(string searchQuery, int page, int limit, string sort)
        {
            var products = await _productService.GetProductsAsync(searchQuery, page, limit, sort);
            return Ok(products);
        }

        [Route("admin/product/totalPages")]
        public async Task<IActionResult> TotalPages(string searchQuery)
        {
            var products = string.IsNullOrEmpty(searchQuery)
                ? await _productService.GetAllProductAsync()
                : await _productService.FindProductsByNameAsync(searchQuery);

            int totalPages = (int)Math.Ceiling((double)products.Count() / _itemsPerPage);
            return Ok(new { totalPages });
        }

        [HttpGet("totalPages")]
        public async Task<IActionResult> GetTotalPages()
        {
            var totalProducts = await _productService.CountProductAsync();
            var totalPages = (int)Math.Ceiling((double)totalProducts / _itemsPerPage); // itemsPerPage là số sản phẩm mỗi trang

            return Ok(new { totalPages });
        }

        public async Task<IActionResult> CreateProduct()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            var brands = await _brandService.GetBrandsAsync();
            // Lấy URL của hình ảnh từ TempData
            var imageUrl = TempData["UploadedImageUrl"] as string;

            // Tạo model hoặc ViewModel cho view
            var model = new ProductRequest
            {
                // Các thuộc tính khác nếu cần
                Image = imageUrl
            };
            ViewBag.Categories = categories;
            ViewBag.Brands = brands;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductRequest model, IFormFile image)
        {
            if (ModelState.IsValid)
            {
                if (image != null && image.Length > 0)
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", Path.GetFileName(image.FileName));

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    // Lưu đường dẫn hình ảnh vào mô hình
                    model.Image = "/images/" + Path.GetFileName(image.FileName);
                }
                await _productService.AddProductAsync(model);
                return RedirectToAction("List"); // Chuyển hướng đến trang danh sách sản phẩm hoặc chi tiết
            }

            // Trả về view với model và hiển thị thông báo lỗi
            return View("Index");
        }


        [HttpGet("admin/product/edit/{id}")]
        public async Task<IActionResult> EditProduct(Guid id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            // Lấy danh sách các thương hiệu và các thể loại
            var categories = await _categoryService.GetAllCategoriesAsync();
            var brands = await _brandService.GetBrandsAsync();

            // Tạo model cho view
            var productRequest = new ProductRequest
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                Image = product.Image,
                BrandName = product.BrandName, // Ánh xạ tên thương hiệu
                CategoryName = product.CategoryName,
                BrandId = product.BrandId,
                CategoryId = product.CategoryId,
            };

            ViewBag.Categories = categories;
            ViewBag.Brands = brands;
            return View(productRequest);
        }


        [HttpPost("admin/product/edit/{id}")]
        public async Task<IActionResult> EditProduct(Guid id, ProductRequest model)
        {
            if (!ModelState.IsValid)
            {
                // Lấy danh sách lỗi từ ModelState
                var errors = ModelState
                    .Where(ms => ms.Key.StartsWith("ProductRequest") || ms.Key.StartsWith("Name") || ms.Key.StartsWith("Description") ||
                                 ms.Key.StartsWith("Price") || ms.Key.StartsWith("Stock") || ms.Key.StartsWith("Image") ||
                                 ms.Key.StartsWith("CategoryId") || ms.Key.StartsWith("CategoryName") || ms.Key.StartsWith("BrandId") ||
                                 ms.Key.StartsWith("BrandName") || ms.Key.StartsWith("Percentage") || ms.Key.StartsWith("CategoryIds"))
                    .SelectMany(ms => ms.Value.Errors)
                    .Select(error => error.ErrorMessage)
                    .ToList();

                // Xử lý lỗi nếu cần
                foreach (var error in errors)
                {
                    // Ví dụ: ghi log lỗi
                    // _logger.LogError(error);
                }

                // Nếu dữ liệu không hợp lệ, tải lại danh sách thương hiệu và thể loại
                ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
                ViewBag.Brands = await _brandService.GetBrandsAsync();
                return View(model);
            }

            try
            {
                // Cập nhật sản phẩm
                await _productService.UpdateProductAsync(model);

                return RedirectToAction("List"); // Chuyển hướng đến trang Index hoặc trang khác theo yêu cầu
            }
            catch (Exception ex)
            {
                // Xử lý lỗi cập nhật nếu có
                ModelState.AddModelError("", "Lỗi khi cập nhật sản phẩm: " + ex.Message);
                ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
                ViewBag.Brands = await _brandService.GetBrandsAsync();
                return View(model);
            }
        }

		[HttpDelete("admin/product/delete/{id}")]
		public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var result = await _productService.DeleteProductAsync(id);
            if(!result)
            {
                return NotFound(); // Trả về 404 nếu ko tìm thấy sản phẩm 
            }
            return Ok();
        }

    }

}
