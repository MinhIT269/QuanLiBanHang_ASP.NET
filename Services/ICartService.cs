

using Quan_ly_ban_hang.Request;

namespace Quan_ly_ban_hang.Services
{
    public interface ICartService
    {
        Guid GetUserId();

		List<CartRequest> GetCartItems();
        void SaveCartSession(List<CartRequest> cart);
        void AddToCart(Guid productId, int quantity, Guid userId, string name = null, decimal price = 0.0m, string image = null);
        void RemoveFromCart(Guid productId);
        void UpdateCartItem(Guid productId, int quantity);
        decimal GetCartTotal();
        void ClearCart();
    }
}
