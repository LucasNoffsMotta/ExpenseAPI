using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using UnitTests_ExpenseAPI.DTO.ExpensesDTO;
using UnitTests_ExpenseAPI.Repo;

namespace UnitTests_ExpenseAPI
{
   
    [Route("api/[controller]")]
    [ApiController]
    public class ExpensesController : ControllerBase
    {
         private IBaseRepo<Expense> _baseService;
        private readonly ILogger<ExpensesController> _logger;

        public ExpensesController(IBaseRepo<Expense> expenseService, ILogger<ExpensesController> logger)
        {
            _baseService = expenseService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {  
            var expensesList = await _baseService.GetAll(null, "category");
            return Ok(expensesList.Select(e => ExpenseMappings.ExpenseModelToSummaryDTO(e)).ToList());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetByID([FromRoute]int id)
        {
            var expense = await _baseService.GetByID(id);
            if (expense is not null)
            {
                return Ok(ExpenseMappings.ExpenseModelToSummaryDTO(expense));
            }

            return NotFound();     
        }

        [HttpGet("byMonth/{month}")]
        public async Task<ActionResult> GetByMonth([FromRoute] int month)
        {
            if (month < 1 || month > 12) return BadRequest("Invalid month value.");

            try
            {
                var expenses = await _baseService.GetAll(e => e.Date.Month == month, "Category");
                var dtos = expenses.Select(e => ExpenseMappings.ExpenseModelToSummaryDTO(e)).ToList();
                return Ok(dtos);
            }

            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateExpenseDTO dto)
        {
            
            var model = await _baseService.Create(ExpenseMappings.ExpenseDtoToModel(dto));
            return model is null ? BadRequest(dto) : Ok(ExpenseMappings.ExpenseModelToSummaryDTO(model));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            bool success = await _baseService.Delete(id);
            return success ? Ok() : BadRequest();
        }
    }
}
