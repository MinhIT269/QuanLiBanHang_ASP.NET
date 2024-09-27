using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quan_ly_ban_hang.Request;
using Quan_ly_ban_hang.Services;

namespace Quan_ly_ban_hang.Controllers
{
	[Route("cart.html")]
	public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CartController(ICartService cartService, IHttpContextAccessor httpContextAccessor)
        {
            _cartService = cartService;
            _httpContextAccessor = httpContextAccessor;
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
                var sessionCartService = new SessionCartService(_httpContextAccessor);
                return Ok(sessionCartService.GetCartItems());
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
                _cartService.AddToCart(request.ProductId, request.Quantity, request.Name, request.Price, request.Image);
                return Ok("Product added to cart");
            }
            else
            {
                var sessionCartService = new SessionCartService(_httpContextAccessor);
                sessionCartService.AddToCart(request.ProductId,request.Quantity, request.Name, request.Price, request.Image);
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
				var sessionCartService = new SessionCartService(_httpContextAccessor);
				foreach (var request in requests)
				{
					sessionCartService.UpdateCartItem(request.ProductId, request.Quantity);
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
                var sessionCartService = new SessionCartService(_httpContextAccessor);
                sessionCartService.RemoveFromCart(productId);
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
                var sessionCartService = new SessionCartService(_httpContextAccessor);
                var total = sessionCartService.GetCartTotal();
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
                var sessionCartService = new SessionCartService(_httpContextAccessor);
                sessionCartService.ClearCart();
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
				var sessionCartService = new SessionCartService(_httpContextAccessor);
				itemCount = sessionCartService.GetCartItems().Count; // Tương tự cho giỏ hàng session
			}

			return Ok(itemCount);
		}
	}
}
