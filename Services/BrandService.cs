using Microsoft.EntityFrameworkCore;
using Quan_ly_ban_hang.Models;
using Quan_ly_ban_hang.Request;
using System.Drawing.Text;

namespace Quan_ly_ban_hang.Services
{
    public class BrandService : IBrandService
    {
        private readonly DataContext _dataContext;
        
        public BrandService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<List<BrandRequest>> GetBrandsAsync()
        {
            try
            {
                return await _dataContext.Brands
                    .Select(b => new BrandRequest
                    {
                        BrandId = b.BrandId,
                        Name = b.Name,
                        Description = b.Description
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu cần
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
