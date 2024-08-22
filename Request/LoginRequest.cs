using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Quan_ly_ban_hang.Request
{
    public class LoginRequest // Dang nhap
    {
        [Required(ErrorMessage ="Email is required"),MaxLength(35, ErrorMessage = "Max 35 characters allowed.")]
        [DisplayName("Username or Email")]
        public string UserNameorEmail { get; set; }

        [Required]
        [StringLength(20,MinimumLength =5,ErrorMessage ="Max 20 or min 5 character allowed.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

    }
}
