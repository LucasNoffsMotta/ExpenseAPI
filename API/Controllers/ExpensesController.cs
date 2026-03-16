using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UnitTests_ExpenseAPI.Services.Expense;

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
        public async Task<ActionResult> GetAll()
        {  
            var expensesList = await _expenseService.GetAll();
            return Ok(expensesList);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetByID([FromRoute]int id)
        {
            var expense = await _expenseService.GetById(id);
            if (expense is not null)
            {
                return Ok(expense);
            }

            return NotFound();     
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateExpenseDTO dto)
        {
            bool success = await _expenseService.Create(dto);
            return success ? Ok(dto) : BadRequest(dto);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            bool success = await _expenseService.DeleteByID(id);
            return success ? Ok() : BadRequest();
        }
    }
}
