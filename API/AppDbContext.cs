using Microsoft.EntityFrameworkCore;
namespace UnitTests_ExpenseAPI;
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Expense> Expenses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.EnableSensitiveDataLogging(true);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

         modelBuilder.Entity<Expense>().HasData(
            new {ID = 1, Descriptiopn = "Ice Cream", Value = 10.0m, Date = DateOnly.MaxValue},
            new {ID= 2,  Description = "Hamburguer", Value = 25.0m, Date = DateOnly.MaxValue}
        );

    }
}