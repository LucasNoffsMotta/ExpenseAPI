using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Reflection;
using UnitTests_ExpenseAPI.DTO.ExpensesDTO;

namespace UnitTests_ExpenseAPI.Services.Excel
{
    public interface IExcelService
    {
        public DataTable? CreateDataTableFromExpensesDTO(IXLWorksheet sheet, List<SummaryExpenseDTO>? _expenses);

        public IXLWorksheet CreateExcelSheetBasedOnDataTable(DataTable table, IXLWorksheet sheet);

        public DataTable InitiateDataTableBasedOnObjProperties(PropertyInfo[] dataProps, string[] columnsToIgnore);

        public Task<List<CreateExpenseDTO>> GetObjectsFromExcel(XLWorkbook excelData, Type baseModel);

        public Task<XLWorkbook> ExportFullYearWorkbook(XLWorkbook excelData, List<SummaryExpenseDTO> _expenses);

        public void InsertSheetContainingMonthsSummary(XLWorkbook excelData, Dictionary<string, IXLWorksheet> monthTableMap);

        public Task InsertSheetContainingCategoriesSummary(XLWorkbook excelData);

        public IXLWorksheet InsertSumRowForColumn(IXLWorksheet sheet, int lastRow, int column);

        public void InsertBaseSheet(IXLWorkbook book, List<SummaryExpenseDTO> _expenses);
    }
}
