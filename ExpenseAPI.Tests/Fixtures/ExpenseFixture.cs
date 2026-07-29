using System;
using System.Collections.Generic;
using System.Text;
using UnitTests_ExpenseAPI;
using UnitTests_ExpenseAPI.DTO.CategoryDTO;
using UnitTests_ExpenseAPI.DTO.ExpensesDTO;

namespace ExpenseAPI.Tests.Fixtures
{
    public static class ExpenseFixture
    {
        public static Expense DefaultExpense(int id, DateOnly date) => new()
        {
            ID = id,
            Category = CategoryFixture.DefaultCategory,
            CategoryId = 1,
            Value = 10,
            Date = DateOnly.MinValue
        };

        public static List<Expense> DefaultExpenseList => new List<Expense>()
        {
            DefaultExpense(1, new DateOnly(2026, 1, 15)),
            DefaultExpense(2, new DateOnly(2026, 2, 15)),
            DefaultExpense(3, new DateOnly(2026, 3, 15)),
            DefaultExpense(4, new DateOnly(2026, 4, 15)),
            DefaultExpense(5, new DateOnly(2026, 5, 15)),
            DefaultExpense(6, new DateOnly(2026, 6, 15)),
            DefaultExpense(7, new DateOnly(2026, 6, 15)),
            DefaultExpense(8, new DateOnly(2026, 6, 15)),
            DefaultExpense(9, new DateOnly(2026, 9, 15)),
            DefaultExpense(10, new DateOnly(2026, 10, 15)),
            DefaultExpense(11, new DateOnly(2026, 11, 15)),
            DefaultExpense(12, new DateOnly(2026, 12, 15))
        };

        public static CreateExpenseDTO CreateExpenseDTO => new(1, 1, DateOnly.MaxValue);
    }
}
