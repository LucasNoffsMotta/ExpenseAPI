using Microsoft.EntityFrameworkCore;
using UnitTests_ExpenseAPI.DTO.CategoryDTO;
using UnitTests_ExpenseAPI.Mapping;
using UnitTests_ExpenseAPI.Models;

namespace UnitTests_ExpenseAPI.Services.Categories
{
    public class CategoryService : BaseService<Category>
    {
        private AppDbContext _dbContext;

        public CategoryService(AppDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }   
    }
}
