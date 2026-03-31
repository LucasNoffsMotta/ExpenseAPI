using System.ComponentModel.DataAnnotations;

namespace UnitTests_ExpenseAPI.DTO.CategoryDTO
{
    public class CreateCategoryDTO
    {
        [Required]
        [StringLength(100)] public string? Description { get; set; }

        [Required]
        public string? HexadecimalColor { get; set; }   
    }
}
