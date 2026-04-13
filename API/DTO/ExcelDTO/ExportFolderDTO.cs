using System.ComponentModel.DataAnnotations;

namespace UnitTests_ExpenseAPI.DTO.ExcelDTO
{
    public record ExportFolderDTO
    (
        [Required]
        string Path
    );
}
