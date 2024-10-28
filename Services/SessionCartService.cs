using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Quan_ly_ban_hang.Request;

namespace Quan_ly_ban_hang.Services
{
    public class SessionCartService : ISessionCartService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private const string CartSessionKey = "cart"; // làm khóa để lưu và truy xuất dữ liệu giỏ hàng từ session

		public SessionCartService(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }
        private ISession Session => _contextAccessor.HttpContext.Session; // Sử dụng Http.session để truy cập vào session

        public List<CartRequest> GetCartItems()
        {
            var sessionData = Session.GetString(CartSessionKey);
            if (string.IsNullOrEmpty(sessionData))
            {
                return new List<CartRequest>();
            }
            return JsonConvert.DeserializeObject<List<CartRequest>>(sessionData); // Chuyển đổi chuỗi Json thành 1 đối tượng list
        }

        public void SaveCartSession(List<CartRequest> cartRequests)
        {
            var sessionData = JsonConvert.SerializeObject(cartRequests);
            Session.SetString(CartSessionKey, sessionData);
        }

        public void AddToCart(Guid productRequest, int stock, int quantity = 1, string name = null, decimal price = 0.0m, string image =null)
        {
            var cart = GetCartItems();
            var cartItem = cart.Find(p => p.ProductId == productRequest);
            if (cartItem != null)
            {
                cartItem.Quantity = quantity;
            }
            else
            {
                cart.Add(new CartRequest { ProductId = productRequest,Stock = stock, Quantity = quantity, Price = price, Name = name, Image = image });
            }
            SaveCartSession(cart);
        }

		public void RemoveFromCart(Guid Id)
        {
            var cart = GetCartItems();
            var cartItem = cart.Find(p => p.ProductId == Id);
            if (cartItem != null)
            {
                cart.Remove(cartItem);
                SaveCartSession(cart);
            }
        }

        public void UpdateCartItem(Guid productId, int quantity)
        {
            var cart = GetCartItems();
            var cartItem = cart.Find(p => p.ProductId == productId);
            if (cartItem != null)
            {
                if (quantity <= 0)
                {
                    cart.Remove(cartItem);
                }
                else
                {
                    cartItem.Quantity = quantity;
                }
                SaveCartSession(cart);
            }
        }

        public decimal GetCartTotal()
        {
            var cart = GetCartItems();
            return cart.Sum(item => item.Price * item.Quantity);
        }

        public void ClearCart()
        {
            SaveCartSession(new List<CartRequest>());
        }

    }
}
