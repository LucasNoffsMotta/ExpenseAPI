using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Reflection;
using UnitTests_ExpenseAPI.DTO.ExpensesDTO;

namespace UnitTests_ExpenseAPI.Services.Excel
{
    public interface IExcelService
    {
        public DataTable? CreateDataTableFromExpensesDTO(List<SummaryTransactionDTO>? _expenses);
        public IXLWorksheet CreateExcelSheetBasedOnDataTable(DataTable table, IXLWorksheet sheet);
        public DataTable InitiateDataTableBasedOnObjProperties(PropertyInfo[] dataProps, string[] columnsToIgnore);
        public Task<List<CreateTransactionDTO>> GetObjectsFromExcel(XLWorkbook excelData, Type baseModel);
        public Task<XLWorkbook> ExportFullYearWorkbook(XLWorkbook excelData, List<SummaryTransactionDTO> _expenses);
        public XLWorkbook ExportMonthWorkbook(string month, List<SummaryTransactionDTO> _expenses);
        public void InsertSheetContainingMonthsSummary(XLWorkbook excelData, Dictionary<string, IXLWorksheet> monthTableMap);
        public Task InsertSheetContainingCategoriesSummary(XLWorkbook excelData);
        public IXLWorksheet InsertSumRowForColumn(IXLWorksheet sheet, int lastRow, int column);
        public void InsertBaseSheet(IXLWorkbook book, List<SummaryTransactionDTO> _expenses);


        //Table layout methods:
        public void MakeHeader(IXLWorksheet sheet, int row, int column, string value);
        public void PaintCellBackground(IXLCell cell, string? hexaDecimalcolor = null, XLColor? color=null);
    }
}
