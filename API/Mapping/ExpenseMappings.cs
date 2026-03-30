using UnitTests_ExpenseAPI.DTO.ExpensesDTO;

namespace UnitTests_ExpenseAPI;

public static class ExpenseMappings
{
    public static Expense ExpenseDtoToModel(CreateExpenseDTO dto)
    {
        Expense expense = new Expense
        {
            CategoryId = dto.CategoryID,
            Value = dto.Value,
            Date = dto.Date
        };
        return expense;
    }
    public static SummaryExpenseDTO ExpenseModelToSummaryDTO(Expense model)
    {
        return new SummaryExpenseDTO(model.ID, model.Category!.Description, model.Value, model.Date, model.Category.HexadecimalColor);
    }
}