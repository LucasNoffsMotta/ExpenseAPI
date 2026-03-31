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
        private IConfiguration _config;

        public ExcelController(IExpenseService expenseService, IExcelService excelService, IConfiguration config)
        {
            _expenseService = expenseService;
            _excelService = excelService;
            _config = config; 
        }

        [HttpPost]
        public async Task<IActionResult> CreateExcelTable()
        {
      
            var expenses = await _expenseService.GetAll();
            var workBook = new XLWorkbook();
            var workSheet = workBook.Worksheets.Add("main_sheet");
            workSheet =  _excelService.SaveDataIntoExcelSheet(workSheet, expenses);
            workBook.SaveAs(_config.GetSection("BasicReportFilePath").Value);   
            return workSheet != null ? Ok($"New table created") : BadRequest($"Table wasnt created");
        }

        [HttpPost("dataReport")]
        public async Task<IActionResult> CreateCompleteDataAnalytcs()
        {
            var expenses = await _expenseService.GetAll();
            XLWorkbook book = new XLWorkbook();

            try
            {
                book = await _excelService.CreateYearReport(book, expenses);
                book.SaveAs(_config.GetSection("FullReportFilePath").Value);
            }

            catch(Exception ex)
            {
                throw new Exception(ex.Message);
                //return BadRequest(ex.Message);
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
