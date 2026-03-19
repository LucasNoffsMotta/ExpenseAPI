
using System.ComponentModel.DataAnnotations;

namespace UnitTests_ExpenseAPI;
public class Expense
{
    public int ID { get; set; }

    public string? Description { get; set; }

    public decimal Value { get; set; }

    public DateOnly Date { get; set; }
}