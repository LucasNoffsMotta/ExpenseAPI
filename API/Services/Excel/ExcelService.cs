using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using DocumentFormat.OpenXml.Office2010.PowerPoint;
using DocumentFormat.OpenXml.Office2016.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SQLitePCL;
using System.Data;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using UnitTests_ExpenseAPI.DTO.CategoryDTO;
using UnitTests_ExpenseAPI.DTO.ExpensesDTO;
using UnitTests_ExpenseAPI.Models;
namespace UnitTests_ExpenseAPI.Services.Excel
{
    public class ExcelService : IExcelService
    {
        private IBaseService<Category> categoryService;

        public ExcelService(IBaseService<Category> categoryService)
        {
            this.categoryService = categoryService;
        }

        public DataTable? CreateDataTableFromExpensesDTO(IXLWorksheet sheet, List<SummaryExpenseDTO>? _expenses)
        {
            //Temp:
            string[] columnsToIgnoreOnDataTable = { "ID" };
   
            if (_expenses?.Count == 0 || _expenses == null) return null;

            Type type = typeof(SummaryExpenseDTO);
            var columnHeaders = type.GetProperties();
            int tableColumnsRange = columnHeaders.Length;

            DataTable table = InitiateDataTableBasedOnObjProperties(columnHeaders, columnsToIgnoreOnDataTable);


            for (int row = 0; row < _expenses.Count; row++)
            {
                var expense = _expenses[row];
                table.Rows.Add(expense.Descricao, expense.Valor, expense.Data.ToString(), expense.Color);
            }

            if (table.Rows.Count == 0)
            {
                table.Rows.Add("Nenhuma", 0.0m, "xx/xx/xx", "#FFFFFF");
            }

            return table;
        }

        //Ignoring the year here...
        public async Task<XLWorkbook> ExportFullYearWorkbook(XLWorkbook workBook, List<SummaryExpenseDTO> _expenses)
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
                var dt = CreateDataTableFromExpensesDTO(monthSheet, mapItem.Value);
                monthSheet = CreateExcelSheetBasedOnDataTable(dt, monthSheet);
                monthTableMap[mapItem.Key] = monthSheet!;
            }

            InsertBaseSheet(workBook, _expenses);
            InsertSheetContainingMonthsSummary(workBook, monthTableMap);
            await InsertSheetConatiningCategoriesSummary(workBook);

            return workBook;
        }


        public DataTable InitiateDataTableBasedOnObjProperties(PropertyInfo[] dataProps, string[] columnsToIgnore)
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
            //IXLWorksheet sheet = excelData.Worksheets.First();
            //var columnHeaders = baseModel.GetProperties();

            //List<CreateExpenseDTO> expenses = new List<CreateExpenseDTO>();
            //int columnCount = sheet.LastColumnUsed()!.ColumnNumber();
            //int rowCount = sheet.LastRowUsed()!.RowNumber();
            //int firstColumn = 2; //Ignore the ID column..

            ////Ferindo principio SOLID! Nao dependa de implementacoes concretas, e sim de abstracoes...

            ////1st row = Header
            ////2nd row = 1st data row
            //for (int row = 0; row < rowCount - 1; row++)
            //{
            //    string description = sheet.Cell(row + 2, firstColumn).Value.ToString();
            //   // var category = await categoryService.GetCategoryByDescription(description);
            //    decimal value = decimal.Parse(sheet.Cell(row + 2, firstColumn + 1).Value.ToString());
            //    DateTime dt = DateTime.Parse(sheet.Cell(row + 2, firstColumn + 2).Value.ToString());
            //    DateOnly date = DateOnly.FromDateTime(dt);

            //    expenses.Add(new CreateExpenseDTO(
            //        category.ID,
            //        value,
            //        date)
            //    );
            //}

            return new List<CreateExpenseDTO>();
        }

        public IXLWorksheet CreateExcelSheetBasedOnDataTable(DataTable table, IXLWorksheet sheet)
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

                sheet = InsertSumRowForColumn(sheet, sheet.LastRowUsed()!.RowNumber(), 2);
            }


            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return sheet;

        }

        public IXLWorksheet InsertSumRowForColumn(IXLWorksheet sheet, int lastRow, int column)
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

        public async Task InsertSheetConatiningCategoriesSummary(XLWorkbook workbook)
        {
            var categories = await categoryService.GetAll();
            var reportSheet = workbook.AddWorksheet("Relatorio Categorias");

            reportSheet.Cell(1, 1).Value = "Categoria";
            reportSheet.Cell(1, 2).Value = "Total";
            reportSheet.ColumnsUsed().Width = 12;

            int categoryRow = 2;

            foreach (var category in categories)
            {
                reportSheet.Cell(categoryRow, 1).Value = category.Description;
                reportSheet.Cell(categoryRow, 1).Style.Fill.SetBackgroundColor(XLColor.FromHtml(category.HexadecimalColor!));

                reportSheet.Cell(categoryRow, 2).FormulaA1 = $"=SUMIFS('Base'!B:B, 'Base'!A:A, A{categoryRow})";
                reportSheet.Cell(categoryRow, 2).Style.NumberFormat.Format = "R$#,##0.00";

                categoryRow++;
            }
        }

        public void InsertSheetContainingMonthsSummary(XLWorkbook workbook, Dictionary<string, IXLWorksheet> monthTableMap)
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

            reportSheet = InsertSumRowForColumn(reportSheet, reportSheet.LastRowUsed()!.RowNumber(), 2);
            reportSheet.Columns().AdjustToContents();

            reportSheet.LastCell();

            reportSheet.Cell(1, 1).Value = "Mes";
            reportSheet.Cell(1, 1).Style.Font.Bold = true;
        

            reportSheet.Cell(1, 2).Value = "Total gasto";
            reportSheet.Cell(1, 2).Style.Font.Bold = true;

            reportSheet.Column(1).Width = 10.0;
            reportSheet.Column(2).Width = 10.0;
            reportSheet.RecalculateAllFormulas();
            reportSheet.Columns().Width = 12;

            workbook.CalculateMode = XLCalculateMode.Auto;
            workbook.RecalculateAllFormulas();
        }

        public void InsertTotalCategoryPerMonth(IXLWorksheet sheet, int tableStart,int lastRow)
        {
            int headerRow = lastRow + 5;

            sheet.Cell(tableStart, 1).FormulaA1 =
                $"=LET(u, UNIQUE(A2:A{lastRow}), HSTACK(u, SUMIFS(B2:B{lastRow}, A2:A{lastRow}, u)))";

            sheet.Cell(headerRow, 1).Value = "Categoria";
            sheet.Cell(headerRow, 2).Value = "Total";

            int tableStart = headerRow + 1;

            //Como eu vou saber as categorias que existem neste mes e o total de cada uma??
            for(int row = tableStart; row < totalCategories; row++)
            {
                sheet.Cell(row, 1).FormulaA1 = $"=LET(u, UNIQUE(A2:A10), HSTACK(u, SUMIFS(B2:B10, A2:A10, u)))";
            }



        }

        public void InsertBaseSheet(IXLWorkbook book, List<SummaryExpenseDTO> _expenses)
        {
            
            var baseSheet = book.AddWorksheet("Base");

            baseSheet.Cell(1, 1).Value = "Categoria";
            baseSheet.Cell(1, 2).Value = "Valor";
            baseSheet.Cell(1, 3).Value = "Data";

            int row = 2;

            foreach (var expense in _expenses)
            {
                baseSheet.Cell(row, 1).Value = expense.Descricao;
                baseSheet.Cell(row, 2).Value = expense.Valor;
                baseSheet.Cell(row, 2).Style.NumberFormat.Format = "R$#,##0.00";
                baseSheet.Cell(row, 3).Value = expense.Data!.Value.ToDateTime(TimeOnly.MinValue);

                row++;
            }

            baseSheet.Visibility = XLWorksheetVisibility.VeryHidden;
        }
    } 
}
