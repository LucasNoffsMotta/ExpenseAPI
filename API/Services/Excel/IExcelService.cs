using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Reflection;
using UnitTests_ExpenseAPI.DTO.ExpensesDTO;

namespace UnitTests_ExpenseAPI.Services.Excel
{
    public interface IExcelService
    {
        public IXLWorkbook? SaveDataIntoExcelSheet(IXLWorkbook book, IXLWorksheet sheet, List<SummaryExpenseDTO> _expenses);

        public DataTable InitiateDataTable(PropertyInfo[] dataProps);

        public Task<List<CreateExpenseDTO>> GetObjectsFromExcel(XLWorkbook excelData, Type baseModel);

        public Task CreateMonthTable(XLWorkbook excelData, List<SummaryExpenseDTO> _expenses);
    }
}
