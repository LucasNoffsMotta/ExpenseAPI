using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using DocumentFormat.OpenXml.Office2010.PowerPoint;
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

        //TODO:
        //Adaptar este metodo para que apenas adicione abas em um workbook, e retorne este workbook!
        public string SaveDataIntoExcelSheet(IXLWorkbook book, IXLWorksheet sheet, List<SummaryExpenseDTO> _expenses)
        {
            try
            {
                if (_expenses.Count == 0) return string.Empty;

                Type type = typeof(SummaryExpenseDTO);
                var columnHeaders = type.GetProperties();
                int tableColumnsRange = columnHeaders.Length;

                DataTable table = InitiateDataTable(columnHeaders);

                //TODO: Abstrair isso para tipos genericos
                foreach (var expense in _expenses)
                {
                    table.Rows.Add(expense.ID, expense.Description, expense.Value, expense.Date.ToString());
                }

                //Insert headers on excel sheet
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    sheet.Cell(1, i + 1).Value = table.Columns[i].ColumnName;
                    sheet.Cell(1, i + 1).Style.Font.Bold = true;
                    sheet.Cell(1, i + 1).Style.Font.FontSize = 16;
                    sheet.Column(i + 1).Width = 15;
                }

                //Insert Data
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    for (int j = 0; j < table.Columns.Count; j++)
                    {
                        var obj = table.Rows[i][j];
                        sheet.Cell(i + 2, j + 1).Value = obj.ToString();
                    }
                }

                book.SaveAs(filepath);
            }

            catch(Exception ex)
            {
                return string.Empty;
            }
          
            return filepath;
        }

        //Here map from expense to month
        //Ignoring the year here...
        public async Task CreateMonthTable(XLWorkbook workBook, List<SummaryExpenseDTO> _expenses)
        {
            IXLWorksheet[] sheets = new IXLWorksheet[12];
            Dictionary<string, IXLWorksheet> monthTableMap = new Dictionary<string, IXLWorksheet>();
            Dictionary<string, List<SummaryExpenseDTO>> monthDtoMap = new Dictionary<string, List<SummaryExpenseDTO>>();

            //Gather all data for each month, and then just trhow on the table!
            //Table map: month / table

            for (int i = 1; i < 13; i++)
            {        
                DateOnly date = new DateOnly(2025, i, 1);
                string sheetTitle = date.ToString("MMM");
                var sheet = workBook.AddWorksheet(sheetTitle);
                monthTableMap[sheetTitle] = sheet;
            }

            foreach(SummaryExpenseDTO expense in _expenses)
            {
                var key = expense.Date!.Value.ToString("MMM");

                if (monthDtoMap.ContainsKey(key))
                {
                    monthDtoMap[key].Add(expense);
                }

                else
                {
                    List<SummaryExpenseDTO> dtoList = new List<SummaryExpenseDTO>();
                    dtoList.Add(expense);
                    monthDtoMap[key] = dtoList;
                }
            }

            foreach(KeyValuePair<string, List<SummaryExpenseDTO>> mapItem in monthDtoMap)
            {
                foreach(var dto in mapItem.Value)
                {

                }
            }



        }


        public DataTable InitiateDataTable(PropertyInfo[] dataProps)
        {
            DataTable table = new DataTable();
            foreach(var prop in dataProps)
            {
                try
                {
                    table.Columns.Add(prop.Name, prop.PropertyType);
                }

                catch(NotSupportedException)
                {
                    table.Columns.Add(prop.Name, typeof(string));
                }
          
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
