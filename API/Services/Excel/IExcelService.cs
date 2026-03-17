using System.Data;
using System.Reflection;

namespace UnitTests_ExpenseAPI.Services.Excel
{
    public interface IExcelService
    {
        public string CreateExcelTable(List<SummaryExpenseDTO> _expenses);

        public DataTable InitiateTable(PropertyInfo[] dataProps);


    }
}
