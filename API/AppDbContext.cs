using Microsoft.EntityFrameworkCore;
using UnitTests_ExpenseAPI.Models;
namespace UnitTests_ExpenseAPI;
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Transaction> Expenses { get; set; }

    public DbSet<Category> Category { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.EnableSensitiveDataLogging(true);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Category>().HasData(
            new { ID = 1, Description = "Ifood", HexadecimalColor = "#FF0000" },
             new { ID = 2, Description = "AppleStore", HexadecimalColor = "#eb4034" }
            );

         modelBuilder.Entity<Transaction>().HasData(
            new {ID = 1, CategoryId = 1, Value = 10.0m, Date = DateOnly.MaxValue},
            new {ID= 2, CategoryId = 2, Value = 25.0m, Date = DateOnly.MaxValue}
        );

    }
}