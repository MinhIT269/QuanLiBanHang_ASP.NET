using System.ComponentModel.DataAnnotations;

namespace Quan_ly_ban_hang.Request
{
	public class RegisterRequest // Chuc nang dang ky
	{
		[Required(ErrorMessage = "Email is required"), MaxLength(35, ErrorMessage = "Max 35 characters allowed.")]
		[EmailAddress]
		public string Email { get; set; }
		[Required]
		[DataType(DataType.Password)]
		[StringLength(20, MinimumLength = 8)]
		[RegularExpression(@"(?=.*[!@#$%^&*()_+{}\[\]:;""'<>,.?/\\|`~]).*", ErrorMessage = "Password must contain at least one special character.")]
		public string Password { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[Compare("Password", ErrorMessage = "Mật khẩu không đúng.")]
		public string ConfirmPassword { get; set; }

		[Required]
		[StringLength(50, MinimumLength = 7)]
		[RegularExpression(@"^[\p{L}\s]+$", ErrorMessage = "Full Name cannot contain numbers and must only include letters and spaces.")]
		public string FullName { get; set; }

		[Required]
		[StringLength(25, MinimumLength = 5)]
	    [RegularExpression(@"^[a-zA-Z0-9_\-\.]*$", ErrorMessage = "User Name can only contain letters, numbers, underscores, hyphens, and dots.")]
		public string UserName { get; set; }

		[Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
		[StringLength(10, MinimumLength = 10, ErrorMessage = "Số điện thoại phải có đúng 10 chữ số.")]
		[Phone(ErrorMessage = "Số điện thoại không đúng định dạng.")]
		public string Phone { get; set; }


	}
}
