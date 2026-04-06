using UnitTests_ExpenseAPI.DTO.CategoryDTO;
using UnitTests_ExpenseAPI.Models;

namespace UnitTests_ExpenseAPI.Mapping
{
    public static class CategoryMapping
    {
        public static Category CategoryDtoToModel(CreateCategoryDTO dto)
        {
            return new Category
            {
                Description = dto.Description,
                HexadecimalColor = dto.HexadecimalColor
            };
        }

        public static SumaryCategoryDTO CategoryModelToSummaryDTO(Category model)
        {
            return new SumaryCategoryDTO
            {
                ID = model.ID,
                Description = model.Description,
                HexadecimalColor = model.HexadecimalColor
            };
        }
    }
}
