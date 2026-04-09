using Microsoft.EntityFrameworkCore;
using UnitTests_ExpenseAPI;
using UnitTests_ExpenseAPI.Models;
using UnitTests_ExpenseAPI.Services;
using UnitTests_ExpenseAPI.Services.Excel;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                       ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString)); // Use Sqlite extension

builder.Services.AddScoped<IBaseService<Category>, BaseService<Category>>();
builder.Services.AddScoped<IBaseService<Expense>, BaseService<Expense>>();
builder.Services.AddScoped<IExcelService, ExcelService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
