namespace UnitTests_ExpenseAPI;

public class SummaryExpenseDTO
{
    public decimal Value { get; set; }
    public string FormattedDate { get; set; }

    public SummaryExpenseDTO(decimal value, DateOnly date)
    {
        Value = value;
        FormattedDate = date.ToShortDateString();
    }
}

