using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quan_ly_ban_hang.Models
{
    public class Customer
    {
        [Key]
        public Guid CustomerId { get; set; }

        [Required]
        public Guid UserId { get; set; }
        [Required,StringLength(50)]
        public required string Address { get; set; }
        public DateTime RegistrationDate { get; set; }

        public User User { get; set; }
        public ICollection<Order> ?Order {  get; set; }
        public ICollection<Review> ?Reviews { get; set; }
        public ShoppingCart ?ShoppingCart { get; set; }
    }
}
