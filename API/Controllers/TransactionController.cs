using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using UnitTests_ExpenseAPI.DTO.ExpensesDTO;
using UnitTests_ExpenseAPI.Repo;

namespace UnitTests_ExpenseAPI
{
   
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
         private IBaseRepo<Transaction> _baseService;
        private readonly ILogger<TransactionController> _logger;

        public TransactionController(IBaseRepo<Transaction> expenseService, ILogger<TransactionController> logger)
        {
            _baseService = expenseService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {  
            var expensesList = await _baseService.GetAll(null, "category");
            return Ok(expensesList.Select(e => TransactionMappings.TransactionModelToSummaryDTO(e)).ToList());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetByID([FromRoute]int id)
        {
            var expense = await _baseService.GetByID(id);
            if (expense is not null)
            {
                return Ok(TransactionMappings.TransactionModelToSummaryDTO(expense));
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
                var dtos = expenses.Select(e => TransactionMappings.TransactionModelToSummaryDTO(e)).ToList();
                return Ok(dtos);
            }

            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateTransactionDTO dto)
        {
            
            var model = await _baseService.Create(TransactionMappings.TransactionDtoToModel(dto));
            return model is null ? BadRequest(dto) : Ok(TransactionMappings.TransactionModelToSummaryDTO(model));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            bool success = await _baseService.Delete(id);
            return success ? Ok() : BadRequest();
        }
    }
}
