using System.ComponentModel.DataAnnotations;
namespace UnitTests_ExpenseAPI.DTO.ExpensesDTO;
public record CreateExpenseDTO(
    [Required] string Description,
    [Required] decimal Value, 
    [Required] DateOnly Date
    );