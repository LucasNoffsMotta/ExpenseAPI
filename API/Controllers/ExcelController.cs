using Microsoft.AspNetCore.Mvc;

namespace UnitTests_ExpenseAPI.Controllers
{
    public class ExcelController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
