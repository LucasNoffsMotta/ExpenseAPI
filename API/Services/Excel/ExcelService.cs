using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using DocumentFormat.OpenXml.Office2010.PowerPoint;
using DocumentFormat.OpenXml.Office2016.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Reflection;
using UnitTests_ExpenseAPI.DTO.CategoryDTO;
using UnitTests_ExpenseAPI.DTO.ExpensesDTO;
using UnitTests_ExpenseAPI.Services.Categories;
namespace UnitTests_ExpenseAPI.Services.Excel
{
    public class ExcelService : IExcelService
    {
        private ICategoryService categoryService;

        public ExcelService(ICategoryService categoryService)
        {
            this.categoryService = categoryService;
        }

        public IXLWorksheet? SaveDataIntoExcelSheet(IXLWorksheet sheet, List<SummaryExpenseDTO>? _expenses)
        {
            //Temp:
            string[] columnsToIgnoreOnDataTable = { "ID" };
   
            if (_expenses?.Count == 0 || _expenses == null) return null;

            Type type = typeof(SummaryExpenseDTO);
            var columnHeaders = type.GetProperties();
            int tableColumnsRange = columnHeaders.Length;

            DataTable table = InitiateDataTable(columnHeaders, columnsToIgnoreOnDataTable);


            for (int row = 0; row < _expenses.Count; row++)
            {
                var expense = _expenses[row];
                table.Rows.Add(expense.Descricao, expense.Valor, expense.Data.ToString(), expense.Color);
            }

            if (table.Rows.Count == 0)
            {
                table.Rows.Add("Nenhuma", 0.0m, "xx/xx/xx", "#FFFFFF");
            }

            sheet = CreateExcelSheetUsingDataTable(table, sheet);

            return sheet;
        }

        //Ignoring the year here...
        public async Task<XLWorkbook> CreateYearReport(XLWorkbook workBook, List<SummaryExpenseDTO> _expenses)
        {
            IXLWorksheet[] sheets = new IXLWorksheet[12];
            Dictionary<string, IXLWorksheet> monthTableMap = new Dictionary<string, IXLWorksheet>();
            Dictionary<string, List<SummaryExpenseDTO>> monthDtoMap = new Dictionary<string, List<SummaryExpenseDTO>>();

            for (int i = 1; i < 13; i++)
            {
                DateOnly date = new DateOnly(2025, i, 1);
                string sheetTitle = date.ToString("MMM");
                var monthSheet = workBook.AddWorksheet(sheetTitle);
                monthTableMap[sheetTitle] = monthSheet;

            }

            foreach (SummaryExpenseDTO expense in _expenses)
            {
                var key = expense.Data!.Value.ToString("MMM");

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

            foreach (KeyValuePair<string, List<SummaryExpenseDTO>> mapItem in monthDtoMap)
            {
                var monthSheet = monthTableMap[mapItem.Key];
                monthSheet = SaveDataIntoExcelSheet(monthSheet, mapItem.Value);
                monthTableMap[mapItem.Key] = monthSheet!;
            }

            workBook = InsertFullYearSheet(workBook, monthTableMap);
            await InsertCategoryReportSheet(workBook);

            return workBook;
        }


        public DataTable InitiateDataTable(PropertyInfo[] dataProps, string[] columnsToIgnore)
        {
            DataTable table = new DataTable();

            foreach (var prop in dataProps)
            {
                try
                {
                    bool ignoreProp = columnsToIgnore.Any(e => e == prop.Name);

                    if (!ignoreProp)
                    {
                        table.Columns.Add(prop.Name, prop.PropertyType);
                    }
                }

                catch (NotSupportedException)
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

        public IXLWorksheet CreateExcelSheetUsingDataTable(DataTable table, IXLWorksheet sheet)
        {
            string[] columnsToIgnoreOnExcel = { "ID", "Color" };

            try
            {
                #region Insert Headers     
                
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    bool ignore = columnsToIgnoreOnExcel.Any(e => e == table.Columns[i].ColumnName);

                    if (!ignore)
                    {
                        sheet.Cell(1, i + 1).Value = table.Columns[i].ColumnName;
                        sheet.Cell(1, i + 1).Style.Font.Bold = true;
                        sheet.Cell(1, i + 1).Style.Font.FontSize = 16;
                        sheet.Row(1).Style.Fill.SetBackgroundColor(XLColor.AshGrey);
                        sheet.Column(i + 1).Width = 15;
                    }
                }
                #endregion

                #region Insert Data

                for (int i = 0; i < table.Rows.Count; i++)
                {
                    for (int j = 0; j < table.Columns.Count; j++)
                    {
                        var obj = table.Rows[i][j];

                        if (!table.Columns[j].ColumnName.Equals("Color"))
                        {
                            Type columnType = table.Columns[j].DataType;


                            if (columnType == typeof(decimal))
                            {
                                sheet.Cell(i + 2, j + 1).Value = (decimal)obj;
                                sheet.Cell(i + 2, j + 1).Style.NumberFormat.Format = "R$#,##0.00";
                            }

                            else
                            {
                                sheet.Cell(i + 2, j + 1).Value = (string)obj;
                            }
                        }

                        else
                        {
                            string color = obj.ToString()!;
                            sheet.Row(i + 2).Style.Fill.BackgroundColor = XLColor.FromHtml(color);
                        }
                    }
                }
                #endregion

                sheet = InsertSumOnColumn(sheet, sheet.LastRowUsed()!.RowNumber(), 2);
            }


            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return sheet;

        }

        public IXLWorksheet InsertSumOnColumn(IXLWorksheet sheet, int lastRow, int column)
        {
            int newRow = lastRow + 1;
            int sumColumn = column;
            int descriptionColumn = column - 1;


            sheet.Cell(newRow, descriptionColumn).Value = "Total";
            sheet.Cell(newRow, descriptionColumn).Style.Font.Bold = true;
            sheet.Cell(newRow, sumColumn).Style.NumberFormat.Format = "R$#,##0.00";
            sheet.Cell(newRow, sumColumn).FormulaA1 = $"SUM(B2:B{lastRow})";

            var totalCell = sheet.Cell(newRow, sumColumn);

            string namedRange = $"Total_{sheet.Name}";
            sheet.Workbook.DefinedNames.Add(namedRange, totalCell.AsRange());

            return sheet;
        }

        public async Task<XLWorkbook> InsertCategoryReportSheet(XLWorkbook workbook)
        {
            var categories = await categoryService.GetAll();
            var reportSheet = workbook.AddWorksheet("Relatorio Categorias");

            reportSheet.Cell(1, 1).Value = "Categoria";
            reportSheet.Cell(1, 2).Value = "Total";

            int row = 2;

            foreach (var category in categories)
            {
                reportSheet.Cell(row, 1).Value = category.Description;
                reportSheet.Cell(row, 1).Style.Fill.SetBackgroundColor(XLColor.FromHtml(category.HexadecimalColor!));
                row++;
            }

            return new XLWorkbook();
        }

        public XLWorkbook InsertFullYearSheet(XLWorkbook workbook, Dictionary<string, IXLWorksheet> monthTableMap)
        {
            var reportSheet = workbook.AddWorksheet("Relatorio Anual");

            int row = 2;

            foreach (var item in monthTableMap)
            {
                reportSheet.Cell(row, 1).Value = item.Key;
                reportSheet.Cell(row, 2).FormulaA1 = $"=Total_{item.Key}";
                reportSheet.Cell(row, 2).Style.NumberFormat.Format = "R$#,##0.00";
                row++;
            }

            reportSheet = InsertSumOnColumn(reportSheet, reportSheet.LastRowUsed()!.RowNumber(), 2);
            reportSheet.Columns().AdjustToContents();

            reportSheet.LastCell();

            reportSheet.Cell(1, 1).Value = "Mes";
            reportSheet.Cell(1, 1).Style.Font.Bold = true;

            reportSheet.Cell(1, 2).Value = "Total gasto";
            reportSheet.Cell(1, 2).Style.Font.Bold = true;

            reportSheet.Column(1).Width = 10.0;
            reportSheet.Column(2).Width = 10.0;
            reportSheet.RecalculateAllFormulas();
            workbook.CalculateMode = XLCalculateMode.Auto;
            workbook.RecalculateAllFormulas();
            return workbook;
        }
    } 
}
