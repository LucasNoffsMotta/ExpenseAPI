using System.ComponentModel.DataAnnotations;
namespace UnitTests_ExpenseAPI;
public record CreateExpenseDTO(
    [Required] decimal Value, 
    [Required] DateOnly Date
    );