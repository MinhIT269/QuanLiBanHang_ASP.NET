using Microsoft.EntityFrameworkCore;
using Quan_ly_ban_hang.Models;
using Quan_ly_ban_hang.Request;
using System.Security.Claims;

namespace Quan_ly_ban_hang.Services
{
    public class CartService : ICartService
    {
        private readonly DataContext _context;
        private readonly Guid _userId;

        public CartService(DataContext context, IHttpContextAccessor httpContextAccessor) {
            _context = context;
            _userId = Guid.Parse(httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value); // Lấy user hiện tại
        }

        public List<CartRequest> GetCartItems()
        {
            // Lấy ShoppingCart dựa trên UserId
            var shoppingCart = _context.ShoppingCarts.FirstOrDefault(sc => sc.Customer.CustomerId == _userId);
            if (shoppingCart == null) // Khong tim thay gio hang tra ve danh sach rong
            {
                return new List<CartRequest>();
            }
            // lay cac cartItem tu shoppingCart
            var cartItems = _context.CartItems.Where(c => c.CartId == shoppingCart.CartId).Select(c => new CartRequest
            {
                ProductId = c.ProductId,
                Quantity = c.Quantity,
            }).ToList();
            return cartItems;
        }

        // Thêm sản phẩm vào giỏ hàng
        public void AddToCart(Guid productId, int quantity, string name = null, decimal price = 0.0m, string image = null)
        {
            // Lấy ShoppingCart của người dùng
            var shoppingCart = _context.ShoppingCarts.FirstOrDefault(sc => sc.Customer.UserId == _userId);

            // Nếu giỏ hàng chưa tồn tại, tạo mới
            if (shoppingCart == null)
            {
                shoppingCart = new ShoppingCart
                {
                    CartId = Guid.NewGuid(),
                    CustomerId = _userId,
                    CreatedDate = DateTime.Now
                };
                _context.ShoppingCarts.Add(shoppingCart);
                _context.SaveChanges();
            }

            // Kiểm tra sản phẩm đã tồn tại trong giỏ hàng hay chưa
            var cartItem = _context.CartItems
                .FirstOrDefault(c => c.CartId == shoppingCart.CartId && c.ProductId == productId);

            if (cartItem != null)
            {
                // Nếu sản phẩm đã tồn tại, cập nhật số lượng
                cartItem.Quantity += quantity;
            }
            else
            {
                // Nếu sản phẩm chưa có trong giỏ, thêm mới
                cartItem = new CartItem
                {
                    CartItemId = Guid.NewGuid(),
                    CartId = shoppingCart.CartId,
                    ProductId = productId,
                    Quantity = quantity
                };
                _context.CartItems.Add(cartItem);
            }

            // Lưu thay đổi vào database
            _context.SaveChanges();
        }

        // Cập nhật số lượng sản phẩm trong giỏ hàng
        public void UpdateCartItem(Guid productId, int quantity)
        {
            var shoppingCart = _context.ShoppingCarts.FirstOrDefault(sc => sc.Customer.UserId == _userId);

            if (shoppingCart != null)
            {
                var cartItem = _context.CartItems
                    .FirstOrDefault(c => c.CartId == shoppingCart.CartId && c.ProductId == productId);

                if (cartItem != null)
                {
                    if (quantity > 0)
                    {
                        cartItem.Quantity = quantity;
                    }
                    else
                    {
                        _context.CartItems.Remove(cartItem);
                    }

                    _context.SaveChanges();
                }
            }
        }

        // Xóa sản phẩm khỏi giỏ hàng
        public void RemoveFromCart(Guid productId)
        {
            var shoppingCart = _context.ShoppingCarts.FirstOrDefault(sc => sc.Customer.UserId == _userId);

            if (shoppingCart != null)
            {
                var cartItem = _context.CartItems
                    .FirstOrDefault(c => c.CartId == shoppingCart.CartId && c.ProductId == productId);

                if (cartItem != null)
                {
                    _context.CartItems.Remove(cartItem);
                    _context.SaveChanges();
                }
            }
        }

        // Tính tổng giá trị giỏ hàng
        public decimal GetCartTotal()
        {
            var shoppingCart = _context.ShoppingCarts.FirstOrDefault(sc => sc.Customer.UserId == _userId);

            if (shoppingCart != null)
            {
                var total = _context.CartItems
                    .Where(c => c.CartId == shoppingCart.CartId)
                    .Sum(c => c.Product.Price * c.Quantity);

                return total;
            }

            return 0;
        }
        public void ClearCart()
        {
            var shoppingCart = _context.ShoppingCarts.FirstOrDefault(sc => sc.Customer.UserId == _userId);

            if (shoppingCart != null)
            {
                // Lấy tất cả các item trong giỏ hàng và xóa chúng
                var cartItems = _context.CartItems.Where(c => c.CartId == shoppingCart.CartId).ToList();
                _context.CartItems.RemoveRange(cartItems);
                _context.SaveChanges();
            }
        }
        public void SaveCartSession(List<CartRequest> requests)
        {

        }
    }
}
