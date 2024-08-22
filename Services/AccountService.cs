using Microsoft.AspNetCore.Identity;
using Quan_ly_ban_hang.Request;
using Quan_ly_ban_hang.Models;
using Quan_ly_ban_hang.Response;
using Microsoft.EntityFrameworkCore;
namespace Quan_ly_ban_hang.Services
{
	public class AccountService : IAccountService
	{
		private readonly DataContext _context;
		public AccountService(DataContext context)
		{
			_context = context;
		}
		public async Task<List<User>> GetAllUsersAsync()
		{
			return await _context.Users.ToListAsync();
		}
		public async Task<bool> IsEmailRegistered(string email)
		{
			return await _context.Users.AnyAsync(u => u.EmailAddress == email);
		}
		public async Task<bool> IsPhoneRegistered(string phone)
		{
			return await _context.Users.AnyAsync(u => u.Phone == phone);
		}
		public async Task<bool> IsUserNameRegistered(string name)
		{
			return await _context.Users.AnyAsync(u => u.UserName == name);
		}
		public async Task<string> RegisterUserAsync(RegisterRequest model)
		{
			if (await IsPhoneRegistered(model.Phone))
			{
				throw new InvalidOperationException("Phone number is already registered.");
			}
			else if (await IsEmailRegistered(model.Email))
			{
				throw new InvalidOperationException("Email is already registered.");
			}
			else if (await IsUserNameRegistered(model.UserName))
			{
				throw new InvalidOperationException("Username is already registered.");
			}

			User user = new User // Khởi tạo đối tượng User lưu db
			{
				UserId = Guid.NewGuid(),
				EmailAddress = model.Email,
				Password = model.Password,
				FullName = model.FullName,
				Phone = model.Phone,
				UserName = model.UserName,
				RoleId = Guid.Parse("A8430DA8-B998-4CFC-B4D5-D47BD5C0E5C3")
			};

			_context.Users.Add(user);

			var customer = new Customer
			{
				CustomerId = Guid.NewGuid(),
				UserId = user.UserId,
				Address = "Chua co",
				RegistrationDate = DateTime.Now
			};
			_context.Customers.Add(customer);
			await _context.SaveChangesAsync();
			return $"{user.FullName} register successfully";
		}
        public async Task<User> ValidateUserAsync(LoginRequest model)
        {
            // Giả sử User có một thuộc tính Roles hoặc có thể truy cập thông tin vai trò qua liên kết
            return await _context.Users
                .Include(u => u.Role) // Bao gồm thông tin vai trò nếu cần
                .SingleOrDefaultAsync(x => (x.EmailAddress == model.UserNameorEmail || x.UserName == model.UserNameorEmail) && x.Password == model.Password);
        }

    }
}
