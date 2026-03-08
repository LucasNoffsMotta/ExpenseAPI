namespace UnitTests_ExpenseAPI;

public static class ExpenseMappings
{
    public static Expense ExpenseDtoToModel(decimal value, DateOnly date)
    {
        Expense expense = new Expense
        {
            Value = value,
            Date = date
        };
        return expense;
    }
    public static SummaryExpenseDTO ExpenseModelToSummaryDTO(Expense model)
    {
        return new SummaryExpenseDTO(model.Value, model.Date);
    }

}