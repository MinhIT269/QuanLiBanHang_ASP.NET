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
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly ICartService _cartService;
		private readonly ISessionCartService _sessionCartService;
		public HomeController(IAccountService accountService, ICategoryService categoryService, IProductService productService, IHttpContextAccessor httpContextAccessor, ICartService cartService, ISessionCartService sessionCartService)
		{
			_accountService = accountService;
			_categoryService = categoryService;
			_productService = productService;
			_httpContextAccessor = httpContextAccessor;
			_cartService = cartService;
			_sessionCartService = sessionCartService;
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
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			// Xác thực người dùng
			var user = await _accountService.ValidateUserAsync(model);
			if (user == null)
			{
				ViewBag.Message = "Invalid username or password.";
				return View(model);
			}

			// Tạo danh sách claims
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, user.EmailAddress),
				new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
				new Claim(ClaimTypes.Role, user.Role.RoleName) // Thêm role nếu cần
            };

			// Tạo đối tượng ClaimsIdentity và đăng nhập người dùng
			var claimIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
			await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
				 new ClaimsPrincipal(claimIdentity));

			// Xử lý giỏ hàng nếu có
			var sessionCartItems = _sessionCartService.GetCartItems();
			if (sessionCartItems.Any())
			{
				foreach (var item in sessionCartItems)
				{
					_cartService.AddToCart(item.ProductId, item.Quantity, user.UserId, item.Name, item.Price, item.Image);
				}
				_sessionCartService.ClearCart();
			}

			// Kiểm tra vai trò của người dùng và chuyển hướng
			var userRole = user.Role?.RoleName; // Lấy role từ đối tượng `user`
			return userRole switch
			{
				"Admin" => RedirectToAction("Index", "Admin"),
				"Customer" => RedirectToAction("Index", "Home"),
				_ => RedirectToAction("Index", "Home"), // Mặc định nếu không có vai trò
			};
		}



		public async Task<IActionResult> LogOut()
		{
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return RedirectToAction("Index");
		}

		[Authorize]
		public IActionResult Securepage()
		{
			ViewBag.Name = User.Identity.Name;
			bool isUser = User.Identity.IsAuthenticated;
			ViewBag.IsUser = isUser;
			return View();
		}
	}
}
