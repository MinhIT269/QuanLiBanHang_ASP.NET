using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quan_ly_ban_hang.Request;
using Quan_ly_ban_hang.Services;
using System.Security.Claims;

namespace Quan_ly_ban_hang.Controllers
{
	[Route("cart.html")]
	public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly ISessionCartService _sessionCartService;
		public CartController(ICartService cartService, IHttpContextAccessor httpContextAccessor, ISessionCartService sessionCartService)
        {
            _cartService = cartService;
            _httpContextAccessor = httpContextAccessor;
            _sessionCartService = sessionCartService;
            
        }
        private bool IsUserAuthenticated()
        {
            return _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet("GetCartItems")]
        public IActionResult GetCartItems()
        {
            if (IsUserAuthenticated())
            {
                return Ok(_cartService.GetCartItems());
            }
            else
            {
                return Ok(_sessionCartService.GetCartItems());
            }
        }

        [HttpPost("AddToCart")]
        public IActionResult AddToCart([FromBody] CartRequest request)
        {

            if (request == null || request.ProductId == Guid.Empty)
            {
                return BadRequest("Invalid request Data");
            }
            if (IsUserAuthenticated())
            {
				var userId = _cartService.GetUserId(); // Lấy userId từ Claims
				_cartService.AddToCart(request.ProductId, request.Quantity, userId, request.Name, request.Price, request.Image);
                return Ok("Product added to cart");
            }
            else
            {
                _sessionCartService.AddToCart(request.ProductId, request.Stock, request.Quantity,request.Name, request.Price, request.Image);
                return Ok("Product added to cart");
            }
        }
        [HttpPut("UpdateCart")]
        public IActionResult UpdateCartItem([FromBody] List<CartRequest> requests)
        {
            if(requests == null || !requests.Any())
            {
                return BadRequest("Invalid request data");
            }
            
            if(IsUserAuthenticated())
            {
				foreach (var request in requests)
				{
					if (request.ProductId == Guid.Empty)
					{
						return BadRequest("Invalid product ID");
					}
					_cartService.UpdateCartItem(request.ProductId, request.Quantity);
				}
				return Ok("Cart Updated.");
			}
            else
            {
				foreach (var request in requests)
				{
					_sessionCartService.UpdateCartItem(request.ProductId, request.Quantity);
				}
				return Ok("Cart Updated.");
			}
        }

        [HttpDelete("Remove/{productId}")]
        public IActionResult RemoveFromCart(Guid productId)
        {
            if (productId == Guid.Empty)
            {
                return BadRequest("Invalid product ID.");
            }

            if (IsUserAuthenticated())
            {
                _cartService.RemoveFromCart(productId);
                return Ok("Product removed from cart.");
            }
            else
            {
       
                _sessionCartService.RemoveFromCart(productId);
                return Ok("Product removed from cart.");
            }
        }

        [HttpGet("Total")]
        public IActionResult GetCartTotal()
        {
            if (IsUserAuthenticated())
            {
                var total = _cartService.GetCartTotal();
                return Ok(total);
            }
            else
            {
                
                var total = _sessionCartService.GetCartTotal();
                return Ok(total);
            }
        }


        [HttpDelete("Clear")]
        public IActionResult ClearCart()
        {
            if (IsUserAuthenticated())
            {
                _cartService.ClearCart();
                return Ok("Cart cleared.");
            }
            else
            {             
                _sessionCartService.ClearCart();
                return Ok("Cart cleared.");
            }
        }
        [HttpGet("CheckCart")]
        public IActionResult CheckCart()
        {
            var session = _httpContextAccessor.HttpContext.Session;
            var cartItems = new List<string>();

            // Lấy tất cả các sản phẩm đã lưu trong session
            foreach (var key in session.Keys)
            {
                cartItems.Add($"{key}: {session.GetString(key)}");
            }

            return Ok(cartItems);
        }
		[HttpGet("CartItemCount")]
		public IActionResult GetCartItemCount()
		{
			int itemCount = 0;

			if (IsUserAuthenticated())
			{
				itemCount = _cartService.GetCartItems().Count; // Phương thức này trả về số lượng sản phẩm trong giỏ
			}
			else
			{
				
				itemCount = _sessionCartService.GetCartItems().Count; // Tương tự cho giỏ hàng session
			}

			return Ok(itemCount);
		}

		[HttpPost("MergeCartOnLogin")]
		public async Task<IActionResult> MergeCartOnLogin()
		{
			if (!IsUserAuthenticated())
			{
				return BadRequest("User not authenticated.");
			}

			// Lấy giỏ hàng từ session
			var sessionCartItems = _sessionCartService.GetCartItems();

			if (sessionCartItems == null || !sessionCartItems.Any())
			{
				return Ok("No items in session cart to merge.");
			}

			// Lấy giỏ hàng từ database của người dùng đã đăng nhập
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			//var userCartItems = await _cartService.GetCartItemsByUserIdAsync(userId);

			// Hợp nhất giỏ hàng
/*			foreach (var sessionItem in sessionCartItems)
			{
				var existingItem = userCartItems.FirstOrDefault(uc => uc.ProductId == sessionItem.ProductId);

				if (existingItem != null)
				{
					// Cập nhật số lượng sản phẩm đã tồn tại trong giỏ của người dùng
					await _cartService.UpdateCartItem(existingItem.ProductId, existingItem.Quantity + sessionItem.Quantity);
				}
				else
				{
					// Thêm sản phẩm mới từ session vào giỏ của người dùng
					await _cartService.AddToCart(sessionItem.ProductId, sessionItem.Quantity, sessionItem.Name, sessionItem.Price, sessionItem.Image);
				}
			}*/

			// Xóa giỏ hàng trong session
			_sessionCartService.ClearCart();

			return Ok("Cart merged successfully after login.");
		}
	}
}
