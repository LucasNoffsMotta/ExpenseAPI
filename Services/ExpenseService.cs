using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
namespace UnitTests_ExpenseAPI;

public class ExpenseService : IExpenseService
{
    private AppDbContext _dbContext;

    public ExpenseService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> Create(Expense model)
    {
        try
        {
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
        throw new NotImplementedException();
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

    public async Task<SummaryExpenseDTO> GetById(int id)
    {
        throw new NotImplementedException();
    }
}

