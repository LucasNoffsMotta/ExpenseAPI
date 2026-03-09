using Microsoft.EntityFrameworkCore;
namespace UnitTests_ExpenseAPI;
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Expense> Expenses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

         modelBuilder.Entity<Expense>().HasData(
            new {ID = 1, Value = 10.0, Date = DateOnly.MaxValue},
            new {ID= 2,  Value = 25.0, Date = DateOnly.MaxValue}
        );
    }
}