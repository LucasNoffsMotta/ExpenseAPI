using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Data;
using System.Reflection;
using UnitTests_ExpenseAPI.DTO.ExpensesDTO;
namespace UnitTests_ExpenseAPI.Services.Excel
{
    public class ExcelService : IExcelService
    {
        const string filepath = @"C:\\Users\\PICHAU\\Desktop\\ExcelExpenses/Expenses.xlsx";

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

        public DataTable InitiateDataTable(PropertyInfo[] dataProps)
        {
            DataTable table = new DataTable();
            foreach(var prop in dataProps)
            {
                table.Columns.Add(prop.Name, prop.PropertyType);
            }

            return table;
        }

        public List<CreateExpenseDTO> GetObjectsFromExcel(XLWorkbook excelData, Type baseModel)
        {
            IXLWorksheet sheet = excelData.Worksheets.First();
            var columnHeaders = baseModel.GetProperties();

            //foreach (IXLColumn column in sheet.ColumnsUsed())
            //{
            //    //Procura por um header na tabela que de match com alguma propriedade do DTO passado:
            //    var currentPropertyName = columnHeaders.Where(h => h.Name == column.FirstCell().Value.ToString()).FirstOrDefault();
                
            //    if (currentPropertyName == null) return null;
            //}

            List<CreateExpenseDTO> expenses = new List<CreateExpenseDTO>();
            int columnCount = sheet.LastColumnUsed()!.ColumnNumber();
            int rowCount = sheet.LastRowUsed()!.RowNumber();

            //Ferindo principio SOLID! Nao dependa de implementacoes concretas, e sim de abstracoes...

            //Sofrivel! Melhorar isso
            for (int i = 0; i < columnCount - 1; i++)
            {
                for (int j = 0; j < rowCount - 1; j++)
                {
                    string description = sheet.Cell(j + 2, i + 1).Value.ToString();
                    decimal value = decimal.Parse(sheet.Cell(j + 2, i + 2).Value.ToString());
                    DateTime dt = DateTime.Parse(sheet.Cell(j + 2, i + 3).Value.ToString());
                    DateOnly date = DateOnly.FromDateTime(dt);


                    expenses.Add(new CreateExpenseDTO(
                        description,
                        value,
                        date)
                    );
                }
                break;
            }

            return expenses;
        }
    }
}
