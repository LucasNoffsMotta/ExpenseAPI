using UnitTests_ExpenseAPI.DTO.ExpensesDTO;

namespace UnitTests_ExpenseAPI;

public static class ExpenseMappings
{
    public static Expense ExpenseDtoToModel(CreateExpenseDTO dto)
    {
        Expense expense = new Expense
        {
            Description = dto.Description,
            Value = dto.Value,
            Date = dto.Date
        };
        return expense;
    }
    public static SummaryExpenseDTO ExpenseModelToSummaryDTO(Expense model)
    {
        return new SummaryExpenseDTO(model.ID, model.Description!, model.Value, model.Date);
    }
}