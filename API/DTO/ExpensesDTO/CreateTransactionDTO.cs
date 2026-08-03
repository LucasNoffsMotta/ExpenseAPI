using System.ComponentModel.DataAnnotations;
namespace UnitTests_ExpenseAPI.DTO.ExpensesDTO;
public record CreateTransactionDTO(
    [Required] int CategoryID,
    [Required] decimal Value, 
    [Required] DateOnly Date
    );