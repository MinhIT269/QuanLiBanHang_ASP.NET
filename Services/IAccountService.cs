
using Quan_ly_ban_hang.Models;
using Quan_ly_ban_hang.Request;

namespace Quan_ly_ban_hang.Services
{
    public interface IAccountService
    {
        Task<bool> IsEmailRegistered(string email);
        Task<bool> IsPhoneRegistered(string phone);
        Task<bool> IsUserNameRegistered(string userName);
        Task<string> RegisterUserAsync(RegisterRequest model);
        Task<List<User>> GetAllUsersAsync();
        Task<User> ValidateUserAsync(LoginRequest model);
    }
}
