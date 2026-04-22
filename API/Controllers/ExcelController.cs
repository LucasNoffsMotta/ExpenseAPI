using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using UnitTests_ExpenseAPI.DTO.ExcelDTO;
using UnitTests_ExpenseAPI.DTO.ExpensesDTO;
using UnitTests_ExpenseAPI.Repo;
using UnitTests_ExpenseAPI.Services.Excel;

namespace UnitTests_ExpenseAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExcelController : Controller
    {
        private IBaseRepo<Expense> _expenseService;
        private IExcelService _excelService;
        private IConfiguration _config;

        public ExcelController(IBaseRepo<Expense> expenseService, IExcelService excelService, IConfiguration config)
        {
            _expenseService = expenseService;
            _excelService = excelService;
            _config = config; 
        }

        [HttpPost]
        public async Task<IActionResult> CreateExcelTable()
        {
      
            var expenses = await _expenseService.GetAll();
            var expensesDTO = expenses.Select(e => ExpenseMappings.ExpenseModelToSummaryDTO(e)).ToList();        
            var workBook = new XLWorkbook();
            var workSheet = workBook.Worksheets.Add("main_sheet");
            var table = _excelService.CreateDataTableFromExpensesDTO(workSheet, expensesDTO);
            var worksheet = _excelService.CreateExcelSheetBasedOnDataTable(table, workSheet);
            workBook.SaveAs(_config.GetSection("BasicReportFilePath").Value);   
            return workSheet != null ? Ok($"New table created") : BadRequest($"Table wasnt created");
        }

        [HttpPost("exportYearReport")]
        public async Task<IActionResult> ExportYearReport([FromBody] ExportFolderDTO exportFolder)
        {
            string filePath = Path.Combine(exportFolder.Path, $"YearReport_{DateTime.Now.Date.ToString("yyyy-MM-dd")}.xlsx");
            var expenses = await _expenseService.GetAll(null, "Category");
            var expensesDTO = expenses.Select(e => ExpenseMappings.ExpenseModelToSummaryDTO(e)).ToList();
            XLWorkbook book = new XLWorkbook();
            ReportExportDTO exportDTO = new ReportExportDTO();

            try
            {
                book = await _excelService.ExportFullYearWorkbook(book, expensesDTO);
                book.SaveAs(filePath);
                exportDTO.Success = true;
                exportDTO.ExportStatus = $"Report exported to {filePath}.";
                exportDTO.FilePath = filePath;
            }

            catch(Exception ex)
            {
                exportDTO.Success = false;
                exportDTO.ExportStatus = ex.Message.ToString();
                exportDTO.FilePath = string.Empty;
            }

            return exportDTO.Success ? Ok(exportDTO) : BadRequest(exportDTO);
        }


        [HttpPost("import")]
        public async Task<IActionResult> ImportTable([FromBody] ImportExcelDTO fileDTO)
        {
            if (!System.IO.File.Exists(fileDTO.DataFile)) return BadRequest();

            try
            {
                XLWorkbook data = new XLWorkbook(fileDTO.DataFile);
                var expenses = await _excelService.GetObjectsFromExcel(data, typeof(CreateExpenseDTO));
                
                //foreach(var expense in expenses)
                //{
                //    await _expenseService.Create(expense);
                //}
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
