namespace UnitTests_ExpenseAPI;

public class SummaryExpenseDTO
{
    public int ID { get; set; }
    public decimal Value { get; set; }
    public string FormattedDate { get; set; }

    public SummaryExpenseDTO(int id, decimal value, DateOnly date)
    {
        ID = id;
        Value = value;
        FormattedDate = date.ToShortDateString();
    }
}

