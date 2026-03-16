using IronXL;
namespace UnitTests_ExpenseAPI.Services.Excel
{
    public class ExcelService : IExcelService
    {
        public string CreateExcelTable(List<SummaryExpenseDTO> _expenses)
        {
            if (_expenses.Count == 0) return "Not created";
            
            Type type = typeof(SummaryExpenseDTO);
            var columnHeaders = type.GetProperties();
            int tableColumnsRange = columnHeaders.Length;

            WorkBook workBook = WorkBook.Create(ExcelFileFormat.XLSX);
            workBook.Metadata.Author = "ExpensesAPI";

            WorkSheet workSheet = workBook.CreateWorkSheet("main_sheet");

            var headers = workSheet.GetRow(1);

            //Create Column Headers
            foreach(var header in columnHeaders)
            {
                foreach(var cell in headers)
                {
                    cell.Value = header.Name;
                    continue;
                }
            }







            

             
            throw new NotImplementedException();
        }
    }
}
