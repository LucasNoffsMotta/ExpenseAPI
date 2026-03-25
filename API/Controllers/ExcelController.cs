using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using UnitTests_ExpenseAPI.DTO.ExcelDTO;
using UnitTests_ExpenseAPI.DTO.ExpensesDTO;
using UnitTests_ExpenseAPI.Services.Excel;
using UnitTests_ExpenseAPI.Services.Expense;

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
            var workBook = new XLWorkbook();
            var workSheet = workBook.Worksheets.Add("main_sheet");
            string tablePath = _excelService.SaveDataIntoExcelSheet(workBook, workSheet, expenses);
            return tablePath != string.Empty ? Ok($"New table created at: {tablePath}") : BadRequest($"Table wasnt created: {tablePath}");
        }

        [HttpPost("dataReport")]
        public async Task<IActionResult> CreateCompleteDataAnalytcs()
        {
            var expenses = await _expenseService.GetAll();
            XLWorkbook book = new XLWorkbook();

            try
            {
                await _excelService.CreateMonthTable(book, expenses);
            }

            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }


        [HttpPost("import")]
        public async Task<IActionResult> ImportTable([FromBody] ImportExcelDTO fileDTO)
        {
            if (!System.IO.File.Exists(fileDTO.DataFile)) return BadRequest();

            try
            {
                XLWorkbook data = new XLWorkbook(fileDTO.DataFile);
                var expenses = await _excelService.GetObjectsFromExcel(data, typeof(CreateExpenseDTO));
                
                foreach(var expense in expenses)
                {
                    await _expenseService.Create(expense);
                }
            }

            //TODO: Criar uma excessao mais especifica para retornar
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(_expenseService.GetAll());
        }
    }
}
