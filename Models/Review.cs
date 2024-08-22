using System.ComponentModel.DataAnnotations;

namespace Quan_ly_ban_hang.Models
{
    public class Review
    {
        [Key]
        public Guid ReviewId { get; set; } // PK

        [Required]
        public Guid ProductId { get; set; } // FK

        [Required]
        public Guid CustomerId { get; set; } // FK

        [Required, Range(1, 5)]
        public int Rating { get; set; }

        public string ?Comment { get; set; }

        public DateTime ReviewDate { get; set; }

        // Navigation properties
        public Product ?Product { get; set; }
        public Customer ?Customer { get; set; }
    }
}
