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

        public IXLWorksheet CreateExcelSheetUsingDataTable(DataTable table, IXLWorksheet sheet);

        public DataTable InitiateDataTable(PropertyInfo[] dataProps, string[] columnsToIgnore);

        public Task<List<CreateExpenseDTO>> GetObjectsFromExcel(XLWorkbook excelData, Type baseModel);

        public Task<XLWorkbook> CreateYearReport(XLWorkbook excelData, List<SummaryExpenseDTO> _expenses);

        public void InsertFullYearSheet(XLWorkbook excelData, Dictionary<string, IXLWorksheet> monthTableMap);

        public Task InsertCategoryReportSheet(XLWorkbook excelData);

        public IXLWorksheet InsertSumOnColumn(IXLWorksheet sheet, int lastRow, int column);

        public void InsertBaseSheet(IXLWorkbook book, List<SummaryExpenseDTO> _expenses);
    }
}
