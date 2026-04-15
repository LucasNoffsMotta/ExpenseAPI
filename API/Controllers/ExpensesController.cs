using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UnitTests_ExpenseAPI.DTO.ExpensesDTO;
using UnitTests_ExpenseAPI.Repo;

namespace UnitTests_ExpenseAPI
{
   
    [Route("api/[controller]")]
    [ApiController]
    public class ExpensesController : ControllerBase
    {
         private IBaseRepo<Expense> _baseService;

        public ExpensesController(IBaseRepo<Expense> expenseService)
        {
            _baseService = expenseService;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {  
            var expensesList = await _baseService.GetAll();
            return Ok(expensesList);
        }

        [HttpGet("summary")]
        public async Task<ActionResult> GetAllSummaryDto()
        {
            var expenses = await _baseService.GetAll(null, "Category");

            List<SummaryExpenseDTO> dtos = new List<SummaryExpenseDTO>();

            foreach(var ex in expenses)
            {
                dtos.Add(
                    new SummaryExpenseDTO
                    (
                        ex.ID, 
                        ex.Category.Description, 
                        ex.Value, 
                        ex.Date, 
                        ex.Category.HexadecimalColor)
                    );
            }

            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetByID([FromRoute]int id)
        {
            var expense = await _baseService.GetByID(id);
            if (expense is not null)
            {
                return Ok(expense);
            }

            return NotFound();     
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateExpenseDTO dto)
        {
            bool success = await _baseService.Create(ExpenseMappings.ExpenseDtoToModel(dto));
            return success ? Ok(dto) : BadRequest(dto);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            bool success = await _baseService.Delete(id);
            return success ? Ok() : BadRequest();
        }
    }
}
