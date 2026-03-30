using Application.DTOs.Requests;
using Application.Interfaces;
using Application.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("")]
    public class ToolsController : ControllerBase
    {
        private readonly IToolService _toolService;
        private readonly IUnitOfWork _unitOfWork;
        public ToolsController(IToolService toolService, IUnitOfWork unitOfWork)
        {
            _toolService = toolService;
            _unitOfWork = unitOfWork;
        }

        [AllowAnonymous]
        [HttpGet("public/tools/available")]
        public async Task<IActionResult> GetAvailableTools(DateTime start, DateTime end)
        {
            var tools = await _toolService.GetAvailableToolsAsync(start, end);
            return Ok(tools);
        }

        [AllowAnonymous]
        [HttpGet("public/tools/filter")]
        public async Task<IActionResult> GetFilteredToolsAsync(ToolFilterRequest filterRequest)
        {
            var tools = await _toolService.FilterToolsAsync(filterRequest);
            return Ok(tools);
        }

        [AllowAnonymous]
        [HttpGet("public/tools/details/{toolId}")]
        public async Task<IActionResult> GetToolDetails([FromRoute] int toolId)
        {
            var toolDetails = await _toolService.GetToolDetailsAsync(toolId);

            if (toolDetails == null)
                return NotFound(new { message = "Tool not found." });

            return Ok(toolDetails);
        }

        // CRUD operations for Admin
        [HttpGet("admin/tools")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllTools()
        {
            var tools = await _toolService.GetAllToolsAsync();
            return Ok(tools);
        }

        [HttpPost("admin/tools")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateTool([FromBody] CreateToolRequest request)
        {
            var result = await _toolService.CreateToolAsync(request);
            await _unitOfWork.SaveChangesAsync();
            return Ok(new { message = result });
        }

        [HttpPut("admin/tools/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateTool(int id, [FromBody] UpdateToolRequest request)
        {
            var result = await _toolService.UpdateToolAsync(id, request);
            
            if (result == "tool not found")
                return NotFound(new { message = result });

            await _unitOfWork.SaveChangesAsync();
            return Ok(new { message = result });
        }

        [HttpDelete("admin/tools/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTool(int id)
        {
            var success = await _toolService.DeleteToolAsync(id);
            
            if (!success)
                return NotFound(new { message = "Tool not found" });

            await _unitOfWork.SaveChangesAsync();
            return Ok(new { message = "Tool deleted successfully" });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin/tools/statistics/general")]
        public async Task<IActionResult> GetGeneralToolStatistics()
        {
            var statistics = await _toolService.GetGeneralToolStatisticsAsync();
            return Ok(statistics);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin/tools/statistics/usage")]
        public async Task<IActionResult> GetToolUsageStatistics()
        {
            var statistics = await _toolService.GetToolUsageStatisticsAsync();
            return Ok(statistics);
        }
    }
}
