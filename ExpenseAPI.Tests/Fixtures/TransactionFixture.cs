using System;
using System.Collections.Generic;
using System.Text;
using UnitTests_ExpenseAPI;
using UnitTests_ExpenseAPI.DTO.CategoryDTO;
using UnitTests_ExpenseAPI.DTO.ExpensesDTO;

namespace ExpenseAPI.Tests.Fixtures
{
    public static class TransactionFixture
    {
        public static Transaction DefaultTransaction(int id, DateOnly date) => new()
        {
            ID = id,
            Category = CategoryFixture.DefaultCategory,
            CategoryId = 1,
            Value = 10,
            Date = date
        };

        public static List<Transaction> DefaultExpenseList => new List<Transaction>()
        {
            DefaultTransaction(1, new DateOnly(2026, 1, 15)),
            DefaultTransaction(2, new DateOnly(2026, 2, 15)),
            DefaultTransaction(3, new DateOnly(2026, 3, 15)),
            DefaultTransaction(4, new DateOnly(2026, 4, 15)),
            DefaultTransaction(5, new DateOnly(2026, 5, 15)),
            DefaultTransaction(6, new DateOnly(2026, 6, 15)),
            DefaultTransaction(7, new DateOnly(2026, 6, 15)),
            DefaultTransaction(8, new DateOnly(2026, 6, 15)),
            DefaultTransaction(9, new DateOnly(2026, 9, 15)),
            DefaultTransaction(10, new DateOnly(2026, 10, 15)),
            DefaultTransaction(11, new DateOnly(2026, 11, 15)),
            DefaultTransaction(12, new DateOnly(2026, 12, 15))
        };

        public static CreateTransactionDTO CreateExpenseDTO => new(1, 1, DateOnly.MaxValue);
    }
}
