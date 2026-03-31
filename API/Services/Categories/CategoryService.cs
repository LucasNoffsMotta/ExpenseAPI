using Microsoft.EntityFrameworkCore;
using UnitTests_ExpenseAPI.DTO.CategoryDTO;
using UnitTests_ExpenseAPI.Mapping;
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

        public async Task<Category> GetCategoryByCategoryId(int categoryId)
        {
            var model = await _dbContext.Category.FirstOrDefaultAsync(c => c.ID == categoryId);
            return model!;
        }

        public async Task<List<SumaryCategoryDTO>> GetAll()
        {
            return await _dbContext.Category.Select(c => CategoryMapping.CategoryModelToSummaryDTO(c)).ToListAsync();
        }

        public async Task<bool> DeleteByID(int ID)
        {
            var del = await _dbContext.Category.Where(e => e.ID == ID).ExecuteDeleteAsync();
            return del != 0;
        }

        public async Task<bool> Create(CreateCategoryDTO dto)
        {
            try
            {
                var checkName = _dbContext.Category.AnyAsync(e => e.Description.Trim().ToLower() == dto.Description.Trim().ToLower());

                if (await checkName == true) return false;

                var model = CategoryMapping.CategoryDtoToModel(dto);
                await _dbContext.Category.AddAsync(model);
            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            var saved = await _dbContext.SaveChangesAsync();
            return saved > 0;
        }
    }
}
