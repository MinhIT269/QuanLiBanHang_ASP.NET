using Azure.Core;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IWebHostEnvironment _env;

        public AdminController(IProductService productService, ICategoryService categoryService, IBrandService brandService, IWebHostEnvironment env)
        {
            _productService = productService;
            _categoryService = categoryService;
            _brandService = brandService;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            return View();
        }
        public async Task<IActionResult> List()
        {
          /*  var products = await _productService.GetAllProductAsync();*/
            return View();
        }

        [Route("admin/product/list")]
        public async Task<IActionResult> ListA(string searchQuery, int page, int limit, string sort) // phục vụ việc lấy danh sách các sản phẩm với các tùy chọn lọc, phân trang, và sắp xếp, rồi trả về danh sách này dưới dạng JSON cho client.
        {
            var products = await _productService.GetProductsAsync(searchQuery, page, limit, sort);
            return Ok(products);
        }

        [Route("admin/product/totalPages")]
        public async Task<IActionResult> TotalPages(string searchQuery) // Tính tổng số trang dựa trên dựa trên 1 danh sách sản phẩm đã lọc
        {
            var products = string.IsNullOrEmpty(searchQuery)
                ? await _productService.GetAllProductAsync()
                : await _productService.FindProductsByNameAsync(searchQuery);

            int totalPages = (int)Math.Ceiling((double)products.Count() / _itemsPerPage);
            return Ok(new { totalPages });
        }

        [HttpGet("totalPages")]
        public async Task<IActionResult> GetTotalPages() // Tính tổng số trang dựa trên toàn bộ sản phẩm csdl
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
        public async Task<IActionResult> CreateProduct(ProductRequest model, IFormFile mainImage, IList<IFormFile> additionalImages)
        {
            if (ModelState.IsValid)
            {
                var imagePaths = new List<string>(); // Danh sách đường dẫn hình ảnh chi tiết

                // Xử lý ảnh chính
                if (mainImage != null && mainImage.Length > 0)
                {   
                    var mainImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", Path.GetFileName(mainImage.FileName));// Xác định đường dẫn lưu trữ cho ảnh chính

                    using (var stream = new FileStream(mainImagePath, FileMode.Create)) // Tạo FileStream để lưu ảnh vào đĩa
                    { 
                        await mainImage.CopyToAsync(stream); // Sao chép dữ liệu từ IFormFile vào FileStream
                    }

                    // Đặt ảnh chính vào danh sách đường dẫn hình ảnh
                    imagePaths.Add("/images/" + Path.GetFileName(mainImage.FileName));
                }
                if (additionalImages != null && additionalImages.Count > 0)
                {
                    foreach (var image in additionalImages)
                    {
                        if (image.Length > 0)
                        {
                            // Xác định đường dẫn lưu trữ hình ảnh
                            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", Path.GetFileName(image.FileName));

                            // Lưu hình ảnh vào đường dẫn
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await image.CopyToAsync(stream);
                            }

                            // Lưu đường dẫn của hình ảnh vào danh sách
                            imagePaths.Add("/images/" + Path.GetFileName(image.FileName));
                        }
                    }

                    // Cập nhật thuộc tính Image với đường dẫn hình ảnh chính
                    model.Image = imagePaths.FirstOrDefault(); // Giả sử hình ảnh đầu tiên là hình ảnh chính

                    // Cập nhật thuộc tính ProductImages với các hình ảnh chi tiết
                    model.ProductImages = imagePaths.Skip(1).Select(path => new ProductImageRequest
                    {
                        ProductImageId = Guid.NewGuid(), // Tạo ID mới cho mỗi hình ảnh
                        ProductId = model.ProductId, // ID của sản phẩm
                        ImageUrl = path
                    }).ToList();
                }

                // Gọi dịch vụ để thêm sản phẩm
                await _productService.AddProductAsync(model);

                // Chuyển hướng đến trang danh sách sản phẩm
                return RedirectToAction("List");
            }

            // Nếu model không hợp lệ, trả lại trang tạo sản phẩm với lỗi
            return View("Index");
        }


        [HttpGet("admin/product/edit/{id}")]
        public async Task<IActionResult> EditProduct(Guid id)
        {
            try
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
                    ProductImages = product.ProductImages
                };

                ViewBag.Categories = categories;
                ViewBag.Brands = brands;
                return View(productRequest);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Có lỗi xảy ra trên máy chủ: " + ex.Message);
            }
            
        }


        [HttpPost("admin/product/edit/{id}")]
        public async Task<IActionResult> EditProduct(Guid id, ProductRequest model, IFormFile mainImage, IList<IFormFile> additionalImages, List<string> oldImageUrls)
        {
            if (mainImage == null || mainImage.Length == 0)
            {
                // Nếu không có ảnh mới, bỏ qua xác thực của trường Image
                ModelState.Remove("Image");
                ModelState.Remove("mainImage");
            }

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
                    Console.WriteLine(error);
                    // Ví dụ: ghi log lỗi
                    // _logger.LogError(error);
                }

                // Nếu dữ liệu không hợp lệ, tải lại danh sách thương hiệu và thể loại
                ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
                ViewBag.Brands = await _brandService.GetBrandsAsync();
                return View(model);
            }

            try
			{   // Cập nhật ảnh chính nếu có
				if (mainImage != null && mainImage.Length > 0)
				{
					var mainImageUrl = await _productService.UploadImageAsync(mainImage);
					// Upload ảnh chính và lưu URL
					model.Image = mainImageUrl;
				}

                // Xử lí ảnh phụ
                var imageUrls = new List<string>();
                // Giữ lại các ảnh phụ cũ nếu có
                if (oldImageUrls != null && oldImageUrls.Count > 0)
                {
                    imageUrls.AddRange(oldImageUrls);
                }

                // Thêm các ảnh phụ mới vào danh sách
                if (additionalImages != null && additionalImages.Count > 0)
                {
                    foreach (var image in additionalImages)
                    {
                        var imageUrl = await _productService.UploadImageAsync(image);
                        imageUrls.Add(imageUrl);
                    }
                }
                // Lưu các ảnh phụ vào model hoặc cơ sở dữ liệu
                model.ProductImages = imageUrls.Select(path => new ProductImageRequest
                {
                    ProductImageId = Guid.NewGuid(), // Tạo ID mới cho mỗi hình ảnh
                    ProductId = model.ProductId, // ID của sản phẩm
                    ImageUrl = path
                }).ToList();
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
            if (!result)
            {
                return NotFound(); // Trả về 404 nếu ko tìm thấy sản phẩm 
            }
            return Ok();
        }

        [HttpPost]
        public ActionResult UploadImage(List<IFormFile> files)
        {
            string filepath = "";

            foreach (IFormFile photo in files)
            {
                // Xác định đường dẫn lưu trữ tệp trên server
                string serverMapPath = Path.Combine(_env.WebRootPath, "images", photo.FileName);

                // Lưu tệp vào đường dẫn
                using (var stream = new FileStream(serverMapPath, FileMode.Create))
                {
                    photo.CopyTo(stream);
                }

                // Đường dẫn trả về
                filepath = "https://localhost:7211/images/" + photo.FileName;
            }

            return Json(new { url = filepath });
        }

    }

}
