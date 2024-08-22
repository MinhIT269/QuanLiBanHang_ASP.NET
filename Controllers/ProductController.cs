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

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Search(string name, Guid cat)
        {
            try
            {
                List<ProductRequest> products;
                if (cat == Guid.Empty)
                {
                    // Tìm sản phẩm theo tên khi không có danh mục cụ thể
                    products = await _productService.FindProductsByNameAsync(name);
                }
                else
                {
                    // Tìm sản phẩm theo tên và danh mục
                    products = await _productService.FindProductsAsync(name, cat);
                }

                ViewBag.Products = products;
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu cần
                ViewBag.ErrorMessage = ex.Message;
            }

            return View();
        }
    }
}
