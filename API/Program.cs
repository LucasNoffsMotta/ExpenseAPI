using Microsoft.EntityFrameworkCore;
using UnitTests_ExpenseAPI;
using UnitTests_ExpenseAPI.Services.Categories;
using UnitTests_ExpenseAPI.Services.Excel;
using UnitTests_ExpenseAPI.Services.Expense;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                       ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString)); // Use Sqlite extension

builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddScoped<IExcelService, ExcelService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
