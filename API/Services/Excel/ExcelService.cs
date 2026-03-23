using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Data;
using System.Reflection;
using UnitTests_ExpenseAPI.DTO.ExpensesDTO;
using UnitTests_ExpenseAPI.Services.Categories;
namespace UnitTests_ExpenseAPI.Services.Excel
{
    public class ExcelService : IExcelService
    {
        const string filepath = @"C:\\Users\\PICHAU\\Desktop\\ExcelExpenses/Expenses.xlsx";

        private ICategoryService categoryService;

        public ExcelService(ICategoryService categoryService)
        {
            this.categoryService = categoryService;
        }

        public string SaveDataIntoExcelSheet(List<SummaryExpenseDTO> _expenses)
        {
            try
            {
                if (_expenses.Count == 0) return string.Empty;

                Type type = typeof(SummaryExpenseDTO);
                var columnHeaders = type.GetProperties();
                int tableColumnsRange = columnHeaders.Length;

                var workBook = new XLWorkbook();
                var workSheet = workBook.Worksheets.Add("main_sheet");
                DataTable table = InitiateDataTable(columnHeaders);


                //TODO: Abstrair isso para tipos genericos
                foreach (var expense in _expenses)
                {
                    table.Rows.Add(expense.ID, expense.Description, expense.Value, expense.FormattedDate);
                }

                //Insert headers on excel sheet
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    workSheet.Cell(1, i + 1).Value = table.Columns[i].ColumnName;
                    workSheet.Cell(1, i + 1).Style.Font.Bold = true;
                    workSheet.Cell(1, i + 1).Style.Font.FontSize = 16;
                    workSheet.Column(i + 1).Width = 15;
                }

                //Insert Data
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    for (int j = 0; j < table.Columns.Count; j++)
                    {
                        var obj = table.Rows[i][j];
                        workSheet.Cell(i + 2, j + 1).Value = obj.ToString();
                    }
                }

                workBook.SaveAs(filepath);
            }

            catch(Exception ex)
            {
                return ex.Message;
            }
          
            return filepath;
        }

        //Here map from expense to month
        public async Task CreateMonthTable(XLWorkbook workBook, List<SummaryExpenseDTO> _expenses)
        {
            string[] months = new string[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dec" };
            IXLWorksheet[] sheets = new IXLWorksheet[12];

            Dictionary<IXLWorksheet, List<SummaryExpenseDTO>> expensesPerMonth = new Dictionary<IXLWorksheet, List<SummaryExpenseDTO>>();

            foreach(string month in months)
            {
                var monthWorkSheet = workBook.AddWorksheet(month);
            }

            foreach(var exp in _expenses)
            {
                
            }
        }

        public DataTable InitiateDataTable(PropertyInfo[] dataProps)
        {
            DataTable table = new DataTable();
            foreach(var prop in dataProps)
            {
                table.Columns.Add(prop.Name, prop.PropertyType);
            }

            return table;
        }

        public async Task<List<CreateExpenseDTO>> GetObjectsFromExcel(XLWorkbook excelData, Type baseModel)
        {
            IXLWorksheet sheet = excelData.Worksheets.First();
            var columnHeaders = baseModel.GetProperties();

            List<CreateExpenseDTO> expenses = new List<CreateExpenseDTO>();
            int columnCount = sheet.LastColumnUsed()!.ColumnNumber();
            int rowCount = sheet.LastRowUsed()!.RowNumber();
            int firstColumn = 2; //Ignore the ID column..

            //Ferindo principio SOLID! Nao dependa de implementacoes concretas, e sim de abstracoes...
            //1st row = Header
            //2nd row = 1st data row
            for (int row = 0; row < rowCount - 1; row++)
            {
                string description = sheet.Cell(row + 2, firstColumn).Value.ToString();
                var category = await categoryService.GetCategoryByDescription(description);
                decimal value = decimal.Parse(sheet.Cell(row + 2, firstColumn + 1).Value.ToString());
                DateTime dt = DateTime.Parse(sheet.Cell(row + 2, firstColumn + 2).Value.ToString());
                DateOnly date = DateOnly.FromDateTime(dt);

                expenses.Add(new CreateExpenseDTO(
                    category.ID,
                    value,
                    date)
                );
            }

            return expenses;
        }
    }
}
