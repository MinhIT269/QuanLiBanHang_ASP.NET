
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quan_ly_ban_hang.Request;
using Quan_ly_ban_hang.Services;
using System.Security.Claims;

namespace Quan_ly_ban_hang.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        public HomeController(IAccountService accountService, ICategoryService categoryService, IProductService productService)
        {
            _accountService = accountService;
            _categoryService = categoryService;
            _productService = productService;
        }
        public IActionResult text()
        {
            return View();
        }
        public async Task<IActionResult> Index()
        {
            var users = await _accountService.GetAllUsersAsync();
            var categories = await _categoryService.GetAllCategoriesAsync(); // Lấy tất cả các danh mục từ service

            var allCategories = new List<CategoryRequest> // Tạo đối tượng đại diện cho "All Categories"
			{
                new CategoryRequest
                {
                    CategoryId =Guid.Empty, // Giá trị "Trống" hoặc "Không có giá trị"  00000000-0000-0000-0000-000000000000
					CategoryName = "All Categories"
                }
            };
            var products = await _productService.GetAllProductAsync();
            var limitedPoducts = products.Take(4).ToList();
            allCategories.AddRange(categories);

            ViewBag.Categories = allCategories.ToList();
            ViewBag.Products = limitedPoducts;
            return View(users);
        }


        public IActionResult Registration()
        {
            return View();
        }
        [HttpPost]
       public async Task<IActionResult> Registration(RegisterRequest model)
{
    if (ModelState.IsValid)
    {
        try
        {
            var message = await _accountService.RegisterUserAsync(model);
            ViewBag.Message = message; // Add message to ViewBag
                    return View(); // Redirect to a success page
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message); // Add error message to ModelState
            // Note: No need to add to ViewBag here if you are using alert
        }
    }
    return View(model);
}
		public IActionResult Login()
        {
              // Kiểm tra nếu người dùng hiện tại đã đăng nhập
    if (HttpContext.User.Identity.IsAuthenticated)
    {
        // Nếu đã đăng nhập, chuyển hướng đến trang an toàn
        return RedirectToAction("Securepage");
    }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest model)
        {
            // Kiểm tra xem mô hình có hợp lệ không
            if (ModelState.IsValid)
            {   // Xác thực người dùng 
                var user = await _accountService.ValidateUserAsync(model);
                if (user != null) // Nếu người dùng hợp lệ, tạo cookie
				{
					// Claim là một đối tượng chứa một cặp giá trị. Claim Type (Loại yêu cầu). Claim Value (Giá trị yêu cầu): 
					var claims = new List<Claim>
                     {
                         new Claim(ClaimTypes.Name, user.EmailAddress),
                         new Claim("Name", user.FullName),
                         new Claim(ClaimTypes.Role, user.Role.RoleName),
                     };
                    //  Tạo đối tượng Claim Identity đại diện cho tập hợp các claims từ danh sách các claims
                    var claimIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme); // CookieAuthenticationDefaults.AuthenticationScheme: Ten phuong thuc xac thuc bang cookie 
					// Đăng nhập người dùng bằng cách tạo và lưu cookie
				    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimIdentity));  //ClaimPrincipal: là một lớp đại diện cho người dùng Có thể chứa nhiều claimIdentity
																																		   //ClaimsPrincipal là lớp chính được sử dụng để quản lý thông tin người dùng trong hệ thống và cung cấp phương thức để truy cập claims và identity của người dùng.

					// Chuyển hướng đến trang an toàn sau khi đăng nhập thành côn?g

					// Kiểm tra vai trò của người dùng và chuyển hướng đến trang tương ứng
					if (user.Role.RoleName == "Admin")
					{
						return RedirectToAction("Index", "Admin"); // Chuyển hướng đến AdminController
					}
					else if (user.Role.RoleName == "Customer")
					{
						return RedirectToAction("Index", "Home"); // Chuyển hướng đến HomeController
					}
				}

                else
                {
                    ViewBag.Message = "Invalid username or password.";
                }
            }
            return View(model);
        }
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index");
        }

        [Authorize]
        public IActionResult Securepage()
        {
            ViewBag.Name = HttpContext.User.Identity.Name;
            bool isUser = HttpContext.User.IsInRole("Customer");
            ViewBag.IsUser = isUser;
            return View();
        }
    }
}
