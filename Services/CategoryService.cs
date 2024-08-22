using Microsoft.EntityFrameworkCore;
using Quan_ly_ban_hang.Models;
using Quan_ly_ban_hang.Request;

namespace Quan_ly_ban_hang.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly DataContext _dataContext;
        public CategoryService(DataContext dataContext)
        {
            _dataContext = dataContext;
        } 
        public async Task<List<CategoryRequest>> GetAllCategoriesAsync()
        {
            return await _dataContext.Categories.Select(c => new CategoryRequest
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
            }).OrderBy(c => c.CategoryName).ToListAsync();
        }
    }
}
