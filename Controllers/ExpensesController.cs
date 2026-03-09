using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace UnitTests_ExpenseAPI
{
   
    [Route("api/[controller]")]
    [ApiController]
    public class ExpensesController : ControllerBase
    {
         private IExpenseService _expenseService;

        public ExpensesController(IExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {  
            var expensesList = await _expenseService.GetAll();
            return Ok(expensesList);
        }
    }
}
