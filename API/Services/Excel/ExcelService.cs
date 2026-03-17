using ClosedXML.Excel;
using System.Data;
using System.Reflection;
namespace UnitTests_ExpenseAPI.Services.Excel
{
    public class ExcelService : IExcelService
    {
        const string filepath = @"C:\\Users\\PICHAU\\Desktop\\ExcelExpenses/Expenses.xlsx";

        public string CreateExcelTable(List<SummaryExpenseDTO> _expenses)
        {

            try
            {
                if (_expenses.Count == 0) return string.Empty;

                Type type = typeof(SummaryExpenseDTO);
                var columnHeaders = type.GetProperties();
                int tableColumnsRange = columnHeaders.Length;

                //WorkBook workBook = WorkBook.Create(ExcelFileFormat.XLSX);.
                var workBook = new XLWorkbook();

                var workSheet = workBook.Worksheets.Add("main_sheet");

                //Work with Datatable here!
                DataTable table = InitiateTable(columnHeaders);

                foreach (var expense in _expenses)
                {
                    table.Rows.Add(expense.ID, expense.Value, expense.FormattedDate);
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

        public DataTable InitiateTable(PropertyInfo[] dataProps)
        {
            DataTable table = new DataTable();
            foreach(var prop in dataProps)
            {
                table.Columns.Add(prop.Name, prop.PropertyType);
            }

            return table;
        }
    }
}
