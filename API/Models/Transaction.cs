
using System.ComponentModel.DataAnnotations;
using UnitTests_ExpenseAPI.Models;

namespace UnitTests_ExpenseAPI;
public class Transaction : BaseModel
{
    public Category? Category { get; set; }

    public int CategoryId { get; set; }

    public decimal Value { get; set; }

    public DateOnly Date { get; set; }
}