using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using UnitTests_ExpenseAPI.DTO.ExpensesDTO;
namespace UnitTests_ExpenseAPI.Services.Expense;

public class ExpenseService : IExpenseService
{
    private AppDbContext _dbContext;

    public ExpenseService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> Create(CreateExpenseDTO dto)
    {
        try
        {
            var model = ExpenseMappings.ExpenseDtoToModel(dto);
            await _dbContext.Expenses.AddAsync(model);
        }

        catch(Exception ex)
        {
            throw new Exception(ex.Message);
        }

        var saved = await _dbContext.SaveChangesAsync();

        return saved > 0;
    }

    public async Task<bool> DeleteByID(int id)
    {
        var expense = await _dbContext.Expenses.FindAsync(id);
        if (expense is null) return false;

        await _dbContext.Expenses.
        Where(e => e.ID == id).
        ExecuteDeleteAsync();
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<List<SummaryExpenseDTO>> GetAll()
    {
        var modelList = await _dbContext.Expenses.ToListAsync();
        List<SummaryExpenseDTO> sumaryList = new List<SummaryExpenseDTO>();

        foreach(var model in modelList)
        {
           sumaryList.Add(ExpenseMappings.ExpenseModelToSummaryDTO(model));
        }

        return sumaryList;
    }

    public async Task<SummaryExpenseDTO?> GetById(int id)
    {
        var model =  await _dbContext.Expenses.FindAsync(id);

        return model != null ? ExpenseMappings.ExpenseModelToSummaryDTO(model!) : null;
    }
}

