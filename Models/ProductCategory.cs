using System.ComponentModel.DataAnnotations;

namespace Quan_ly_ban_hang.Models
{
    public class ProductCategory
    {
        [Required]
        public Guid ProductId { get; set; }
        public Product ?Product { get; set; }

        public Guid CategoryId { get; set; }

        public Category ?Category { get; set; }
    }
}
