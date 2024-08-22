using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quan_ly_ban_hang.Models
{
    [Table(name: "Nguoi dung")]
    public class User
    {
        [Key]
        public Guid UserId { get; set; }
        [Required]
        public Guid RoleId { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 7, ErrorMessage = "Phải dài từ 7 đến 20 ký tự")]
        [DataType(DataType.Password)]
        public string ?Password { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        [Required]
        [StringLength(50,MinimumLength = 7)] 
        public string FullName { get; set; }
		[Required]
		[StringLength(25, MinimumLength = 5)]
		public string UserName { get; set; } 
		public Customer Customer { get; set; }
        public Role Role { get; set; }

    }
}
