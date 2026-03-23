using UnitTests_ExpenseAPI.Models;

namespace UnitTests_ExpenseAPI.Services.Categories
{
    public interface ICategoryService
    {
        public Task<Category> GetCategoryByDescription(string description);
    }
}
