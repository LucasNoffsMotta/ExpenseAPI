
namespace UnitTests_ExpenseAPI;

public interface IExpenseService
{
    public Task<List<SummaryExpenseDTO>> GetAll();

    public Task<SummaryExpenseDTO> GetById(int id);

    public Task<bool> DeleteByID(int id);

    public Task<bool> Create(Expense model);

}

