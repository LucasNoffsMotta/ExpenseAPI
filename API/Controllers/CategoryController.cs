using Microsoft.AspNetCore.Mvc;
using UnitTests_ExpenseAPI.DTO.CategoryDTO;
using UnitTests_ExpenseAPI.Services.Categories;

namespace UnitTests_ExpenseAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class CategoryController : Controller
    {
        private ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryService.GetAll();
            return Ok(categories);
        }

        [HttpPost("delete/{ID}")]
        public async Task<IActionResult> DeleteByID([FromRoute] int ID)
        {
            var success = await _categoryService.DeleteByID(ID);
            return success ? Ok() : BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(CreateCategoryDTO dto)
        {
            var result = await _categoryService.Create(dto);
            return Ok(result);
        }
    }
}
