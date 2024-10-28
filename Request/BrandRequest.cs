using System.ComponentModel.DataAnnotations;

namespace Quan_ly_ban_hang.Request
{
    public class BrandRequest
    {
        public Guid BrandId { get; set; }

        public string? Name { get; set; }
        public string Description { get; set; }

    }
}
