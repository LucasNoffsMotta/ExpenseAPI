using UnitTests_ExpenseAPI.DTO.ExpensesDTO;

namespace UnitTests_ExpenseAPI;

public static class TransactionMappings
{
    public static Transaction TransactionDtoToModel(CreateTransactionDTO dto)
    {
        Transaction expense = new Transaction
        {
            CategoryId = dto.CategoryID,
            Value = dto.Value,
            Date = dto.Date
        };
        return expense;
    }
    public static SummaryTransactionDTO TransactionModelToSummaryDTO(Transaction model)
    {
        return new SummaryTransactionDTO(model.ID, model.Category!.Description, model.Value, model.Date, model.Category.HexadecimalColor);
    }
}