using Microsoft.AspNetCore.Mvc;
using UnitTests_ExpenseAPI.Services.Expense;
using UnitTests_ExpenseAPI.Services.Excel;

namespace UnitTests_ExpenseAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExcelController : Controller
    {
        private IExpenseService _expenseService;
        private IExcelService _excelService;

        public ExcelController(IExpenseService expenseService, IExcelService excelService)
        {
            _expenseService = expenseService;
            _excelService = excelService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateExcelTable()
        {
            var expenses = await _expenseService.GetAll();
            string tablePath = _excelService.CreateExcelTable(expenses);
            return tablePath != string.Empty ? Ok($"New table created at: {tablePath}") : BadRequest($"Table wasnt created: {tablePath}");
        }
    }
}
