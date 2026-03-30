namespace UnitTests_ExpenseAPI.DTO.ExpensesDTO;

public class SummaryExpenseDTO
{
    public int ID { get; set; }
    public string? Descricao { get; set; }
    public decimal Valor { get; set; }
    public DateOnly? Data { get; set; }
    public string Color { get; set; }

    public SummaryExpenseDTO(int id, string description, decimal value, DateOnly date, string color)
    {
        ID = id;
        Descricao = description;
        Valor = value;
        Data = date;
        Color = color;
    }
}

