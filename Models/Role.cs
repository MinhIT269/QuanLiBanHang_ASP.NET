using System.ComponentModel.DataAnnotations;

namespace Quan_ly_ban_hang.Models
{
    public class Role
    {
        [Key]
        public Guid RoleId { get; set; }
        [Required, MaxLength(10)]
        public string? RoleName {  get; set; }

        public ICollection<User>? Users { get; set; }
    }
}
