namespace UnitTests_ExpenseAPI.DTO.ExpensesDTO;

public class SummaryExpenseDTO
{
    public int ID { get; set; }
    public string? Description { get; set; }
    public decimal Value { get; set; }
    public string FormattedDate { get; set; }

    public SummaryExpenseDTO(int id, string description, decimal value, DateOnly date)
    {
        ID = id;
        Description = description;
        Value = value;
        FormattedDate = date.ToShortDateString();
    }
}

