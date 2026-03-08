using Microsoft.EntityFrameworkCore;
namespace UnitTests_ExpenseAPI;
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Expense> Expenses { get; set; }


}