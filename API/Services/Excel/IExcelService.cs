using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Reflection;
using UnitTests_ExpenseAPI.DTO.ExpensesDTO;

namespace UnitTests_ExpenseAPI.Services.Excel
{
    public interface IExcelService
    {
        public IXLWorksheet? SaveDataIntoExcelSheet(IXLWorksheet sheet, List<SummaryExpenseDTO>? _expenses);

        public DataTable InitiateDataTable(PropertyInfo[] dataProps, string[] columnsToIgnore);

        public Task<List<CreateExpenseDTO>> GetObjectsFromExcel(XLWorkbook excelData, Type baseModel);

        public Task<XLWorkbook> CreateMonthTable(XLWorkbook excelData, List<SummaryExpenseDTO> _expenses);
    }
}
