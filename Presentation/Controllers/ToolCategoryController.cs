using Microsoft.AspNetCore.Mvc;
using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Interfaces;
using Application.Services;
using Microsoft.AspNetCore.Authorization;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("admin/tool-categories")]
    [Authorize(Roles = "Admin")]
    public class ToolCategoryController : ControllerBase
    {
        private readonly ToolCategoryService _toolCategoryService;
        private readonly IUnitOfWork _unitOfWork;

        public ToolCategoryController(ToolCategoryService toolCategoryService, IUnitOfWork unitOfWork)
        {
            _toolCategoryService = toolCategoryService;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _toolCategoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _toolCategoryService.GetCategoryByIdAsync(id);
            if (category == null)
                return NotFound(new { message = "Category not found." });

            return Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CreateToolCategoryRequest request)
        {
            await _toolCategoryService.CreateCategoryAsync(request);
            await _unitOfWork.SaveChangesAsync();

            return Ok(new { message = "Category created successfully." });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateToolCategoryRequest request)
        {
            var category = await _toolCategoryService.UpdateCategoryAsync(id, request);
            if (category == null)
                return NotFound(new { message = "Category not found." });

            await _unitOfWork.SaveChangesAsync();
            return Ok(category);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var success = await _toolCategoryService.DeleteCategoryAsync(id);
            if (!success)
                return BadRequest(new { message = "Can't delete category." });

            await _unitOfWork.SaveChangesAsync();
            return NoContent();
        }
    }
}
