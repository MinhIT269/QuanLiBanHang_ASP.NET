using Microsoft.AspNetCore.Mvc;
using Quan_ly_ban_hang.Request;
using Quan_ly_ban_hang.Services;
using System;
using System.Threading.Tasks;

namespace Quan_ly_ban_hang.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        public ProductController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }
        public async Task<IActionResult> Index()
        {

            return View();
        }

		[HttpGet]
		public async Task<IActionResult> ProductDetail(Guid id)
		{
			// Truy vấn thông tin chi tiết sản phẩm dựa trên ID
			var product = await _productService.GetProductByIdAsync(id);

			if (product == null)
			{
				return NotFound();
			}
			var categories = await _categoryService.GetAllCategoriesAsync(); // Lấy tất cả các danh mục từ service

			var allCategories = new List<CategoryRequest> // Tạo đối tượng đại diện cho "All Categories"
			{
				new CategoryRequest
				{
					CategoryId =Guid.Empty, // Giá trị "Trống" hoặc "Không có giá trị"  00000000-0000-0000-0000-000000000000
					CategoryName = "All Categories"
				}
			};
			allCategories.AddRange(categories);

			ViewBag.Categories = allCategories.ToList();
			return View(product);
		}
		[Route("product/totalPages")]
        public async Task<IActionResult> TotalPages(string searchQuery, Guid idCat, int itemsPerPage)
        {
            try
			{
				IEnumerable<ProductRequest> products;

				if (idCat == Guid.Empty)
				{
					// Tìm sản phẩm theo tên khi không có danh mục cụ thể
					products = await _productService.FindProductsByNameAsync(searchQuery);
				}
				else
				{
					// Tìm sản phẩm theo tên và danh mục
					products = await _productService.FindProductsAsync(searchQuery, idCat);
				}

				int totalPages = (int)Math.Ceiling((double)products.Count() / itemsPerPage); // Giả sử mỗi trang có 12 sản phẩm
                return Ok(new { totalPages });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        public async Task<IActionResult> Search()
		{
            var categories = await _categoryService.GetAllCategoriesAsync(); // Lấy tất cả các danh mục từ service

            var allCategories = new List<CategoryRequest> // Tạo đối tượng đại diện cho "All Categories"
			{
                new CategoryRequest
                {
                    CategoryId =Guid.Empty, // Giá trị "Trống" hoặc "Không có giá trị"  00000000-0000-0000-0000-000000000000
					CategoryName = "All Categories"
                }
            };
            allCategories.AddRange(categories);

            ViewBag.Categories = allCategories.ToList();
            return View();
		}
		[HttpGet]
        [Route("product/search/list")]
        public async Task<IActionResult> SearchA(string searchQuery, Guid idCat, int page, int limit, string sort = "")
        {
            try
            {
                IEnumerable<ProductRequest> products;

                if (idCat == Guid.Empty)
                {
                    // Tìm sản phẩm theo tên khi không có danh mục cụ thể
                    products = await _productService.FindProductsByNameAsync(searchQuery);
                }
                else
                {
                    // Tìm sản phẩm theo tên và danh mục
                    products = await _productService.FindProductsAsync(searchQuery, idCat);
                }

                // Sắp xếp nếu có tiêu chí sắp xếp
                if (!string.IsNullOrEmpty(sort))
                {
                    // Giả sử sort có thể là "price_asc", "price_desc", etc.
                    switch (sort)
                    {
                        case "price_asc":
                            products = products.OrderBy(p => p.Price).ToList();
                            break;
                        case "price_desc":
                            products = products.OrderByDescending(p => p.Price).ToList();
                            break;
                        case "best_selling":
                            products = products.OrderBy(p=>p.Stock).ToList();
                            break;
                            // Thêm các tiêu chí sắp xếp khác nếu cần
                    }
                }
                // Áp dụng phân trang
                var paginatedProducts = products.Skip((page - 1) * limit).Take(limit).ToList();

                return Ok(paginatedProducts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [HttpGet]
        [Route("product/categories")]
        public async Task<IActionResult> Categories()
        {
            try
            {
                var categories = await _categoryService.GetAllCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
	}
}
