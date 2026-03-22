using System.ComponentModel.DataAnnotations;
namespace UnitTests_ExpenseAPI.DTO.ExpensesDTO;
public record CreateExpenseDTO(
    [Required] int CategoryID,
    [Required] decimal Value, 
    [Required] DateOnly Date
    );