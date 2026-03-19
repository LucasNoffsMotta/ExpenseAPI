using DocumentFormat.OpenXml.Bibliography;
using System.ComponentModel.DataAnnotations;

namespace UnitTests_ExpenseAPI.DTO.ExcelDTO
{
    public record ImportExcelDTO (
            [Required] string DataFile
    );

}
