using System;
using System.Collections.Generic;
using System.Text;
using UnitTests_ExpenseAPI.Models;

namespace ExpenseAPI.Tests.Fixtures
{
    public static class CategoryFixture
    {
        public static Category DefaultCategory => new()
        {
            ID = 1,
            Description = "Test",
            HexadecimalColor = "xxxxx"
        };
    }
}
