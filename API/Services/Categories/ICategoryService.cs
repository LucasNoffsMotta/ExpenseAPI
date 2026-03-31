using UnitTests_ExpenseAPI.DTO.CategoryDTO;
using UnitTests_ExpenseAPI.Models;

namespace UnitTests_ExpenseAPI.Services.Categories
{
    public interface ICategoryService
    {
        public Task<Category> GetCategoryByDescription(string description);

        public Task<Category> GetCategoryByCategoryId(int categoryId);

        public Task<List<SumaryCategoryDTO>> GetAll();

        public Task<bool> Create(CreateCategoryDTO dto);

        public Task<bool> DeleteByID(int ID);
    }
}
