using Quan_ly_ban_hang.Request;

namespace Quan_ly_ban_hang.Services
{
    public interface IBrandService
    {
        Task<List<BrandRequest>> GetBrandsAsync();
    }
}
