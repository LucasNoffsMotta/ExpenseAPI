using Microsoft.AspNetCore.Mvc;
using UnitTests_ExpenseAPI.DTO.CategoryDTO;
using UnitTests_ExpenseAPI.Mapping;
using UnitTests_ExpenseAPI.Models;
using UnitTests_ExpenseAPI.Services;

namespace UnitTests_ExpenseAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class CategoryController : Controller
    {
        private IBaseService<Category> _categoryService;

        public CategoryController(IBaseService<Category> categoryService)
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
            var success = await _categoryService.Delete(ID);
            return success ? Ok() : BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(CreateCategoryDTO dto)
        {
            var result = await _categoryService.Create(CategoryMapping.CategoryDtoToModel(dto));
            return Ok(result);
        }
    }
}
