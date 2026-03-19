using ClosedXML.Excel;
using System.Data;
using System.Reflection;
using UnitTests_ExpenseAPI.DTO.ExpensesDTO;

namespace UnitTests_ExpenseAPI.Services.Excel
{
    public interface IExcelService
    {
        public string SaveDataIntoExcelSheet(List<SummaryExpenseDTO> _expenses);

        public DataTable InitiateDataTable(PropertyInfo[] dataProps);

        public List<CreateExpenseDTO> GetObjectsFromExcel(XLWorkbook excelData, Type baseModel);


    }
}
