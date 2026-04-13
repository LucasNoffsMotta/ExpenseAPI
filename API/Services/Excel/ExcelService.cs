using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Data;
using System.Reflection;
using UnitTests_ExpenseAPI.DTO.ExpensesDTO;
using UnitTests_ExpenseAPI.Models;
using UnitTests_ExpenseAPI.Repo;
namespace UnitTests_ExpenseAPI.Services.Excel
{
    public class ExcelService : IExcelService
    {
        private IBaseRepo<Category> categoryRepo;

        public ExcelService(IBaseRepo<Category> categoryRepo)
        {
            this.categoryRepo = categoryRepo;
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

                var table = monthSheet.Range($"A1:C{monthSheet.LastRowUsed()!.RowNumber() - 1}").CreateTable();
                table.Name = $"TabelaMes_{mapItem.Key}";

                InsertTotalCategoryPerMonth(monthSheet, table, mapItem.Value);
                monthTableMap[mapItem.Key] = monthSheet!;
            }

            InsertBaseSheet(workBook, _expenses);
            InsertSheetContainingMonthsSummary(workBook, monthTableMap);
            await InsertSheetContainingCategoriesSummary(workBook);

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

        //TODO
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
            //   // var category = await categoryRepo.GetCategoryByDescription(description);
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
                        MakeHeader(sheet, 1, i + 1, table.Columns[i].ColumnName);
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

        public async Task InsertSheetContainingCategoriesSummary(XLWorkbook workbook)
        {
            var categories = await categoryRepo.GetAll();
            var reportSheet = workbook.AddWorksheet("Relatorio Categorias");

            MakeHeader(reportSheet, 1, 1, "Categoria");
            MakeHeader(reportSheet, 1, 2, "Total");

            int categoryRow = 2;

            foreach (var category in categories)
            {
                reportSheet.Cell(categoryRow, 1).Value = category.Description;
                reportSheet.Cell(categoryRow, 2).FormulaA1 = $"=SUMIFS('Base'!B:B, 'Base'!A:A, A{categoryRow})";
                reportSheet.Cell(categoryRow, 2).Style.NumberFormat.Format = "R$#,##0.00";
                PaintCellBackground(reportSheet.Cell(categoryRow, 1), category.HexadecimalColor!, null);
                PaintCellBackground(reportSheet.Cell(categoryRow, 2), category.HexadecimalColor!, null);
                categoryRow++;
            }
        }

        public void InsertSheetContainingMonthsSummary(XLWorkbook workbook, Dictionary<string, IXLWorksheet> monthTableMap)
        {      
            var reportSheet = workbook.AddWorksheet("Relatorio Anual");

            MakeHeader(reportSheet, 1, 1, "Mes");
            MakeHeader(reportSheet, 1, 2, "Total Gasto");

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

        }

        private void InsertTotalCategoryPerMonth(IXLWorksheet sheet, IXLTable table, List<SummaryExpenseDTO>? dtoList)
        {
            var categories = 
                dtoList!
                .Select(e => e).DistinctBy(e => e.Descricao)
                .ToList();

           
            int headerRow = sheet.LastRowUsed()!.RowNumber() + 5;
            int startRow = headerRow + 1;

            MakeHeader(sheet, headerRow, 1, "Categoria");
            MakeHeader(sheet, headerRow, 2, "Total");

            int categoryRow = startRow;

            for(int i = 0; i < categories.Count; i++)
            {
                sheet.Cell(categoryRow, 1).Value = categories[i].Descricao;
                PaintCellBackground(sheet.Cell(categoryRow, 1), categories[i].Color!, null);
                PaintCellBackground(sheet.Cell(categoryRow, 2), categories[i].Color!, null);
                categoryRow++;
            }

            for (int row = startRow; row < 50; row++)
            {
                sheet.Cell(row, 2).FormulaA1 =
                    $"=IF(A{row}=\"\",\"\",SUMIFS({table.Name}[Valor], {table.Name}[Descricao], A{row}))";

                sheet.Cell(row, 2).Style.NumberFormat.Format = "R$#,##0.00";
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
                var cell = baseSheet.Cell(row, 3);

                baseSheet.Cell(row, 1).Value = expense.Descricao;
                baseSheet.Cell(row, 2).Value = expense.Valor;
                baseSheet.Cell(row, 3).Value = expense.Data!.Value.ToDateTime(TimeOnly.MinValue);

                row++;
            }

            var table = baseSheet.Range($"A1:C{row - 1}").CreateTable();
            table.Name = "Base"; 

            baseSheet.Visibility = XLWorksheetVisibility.VeryHidden;
        }

        public void MakeHeader(IXLWorksheet sheet, int row, int column, string value)
        {
            sheet.Cell(row, column).Value = value;
            sheet.Cell(row, column).Style.Font.Bold = true;
            sheet.Cell(row, column).Style.Font.FontSize = 16;
            PaintCellBackground(sheet.Cell(row, column), null, XLColor.AshGrey);
            sheet.Row(row).Style.Fill.SetBackgroundColor(XLColor.AshGrey);
            sheet.Column(column).Width = 15;
        }

        public void PaintCellBackground(IXLCell cell, string? hexaDecimalcolor = null, XLColor? color = null)
        {
            if (hexaDecimalcolor != null)
            {
                PaintCellBackground(cell, hexaDecimalcolor);
            }

            else if(color != null)
            {
                PaintCellBackground(cell, color);
            }
        }

        private void PaintCellBackground(IXLCell cell, string hexadecimalColor)
        {
            cell.Style.Fill.SetBackgroundColor(XLColor.FromHtml(hexadecimalColor));
        }

        private void PaintCellBackground(IXLCell cell, XLColor color)
        {
            cell.Style.Fill.SetBackgroundColor(color);
        }
    } 
}
