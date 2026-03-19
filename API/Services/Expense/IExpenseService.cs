using UnitTests_ExpenseAPI.DTO.ExpensesDTO;

namespace UnitTests_ExpenseAPI.Services.Expense;

public interface IExpenseService
{
    public Task<List<SummaryExpenseDTO>> GetAll();

    public Task<SummaryExpenseDTO?> GetById(int id);

    public Task<bool> DeleteByID(int id);

    public Task<bool> Create(CreateExpenseDTO model);

}

