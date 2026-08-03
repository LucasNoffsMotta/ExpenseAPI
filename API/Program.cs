using Microsoft.EntityFrameworkCore;
using UnitTests_ExpenseAPI;
using UnitTests_ExpenseAPI.Models;
using UnitTests_ExpenseAPI.Repo;
using UnitTests_ExpenseAPI.Services.Excel;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                       ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString)); 

builder.Services.AddScoped<IBaseRepo<Category>, BaseRepo<Category>>();
builder.Services.AddScoped<IBaseRepo<Transaction>, BaseRepo<Transaction>>();
builder.Services.AddScoped<IExcelService, ExcelService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); 
}

app.UseAuthorization();
app.MapControllers();
app.Run();
