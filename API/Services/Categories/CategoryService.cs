using Microsoft.EntityFrameworkCore;
using UnitTests_ExpenseAPI.Models;

namespace UnitTests_ExpenseAPI.Services.Categories
{
    public class CategoryService : ICategoryService
    {
        private AppDbContext _dbContext;

        public CategoryService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Category> GetCategoryByDescription(string description)
        {
            description = description.Trim();
            var model = await _dbContext.Category.FirstOrDefaultAsync(c => c.Description == description);
            return model!;
        }
    }
}
