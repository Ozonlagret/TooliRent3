using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Interfaces;
using Application.Interfaces.Service;
using Application.Services;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ToolsController : ControllerBase
    {
        private readonly IToolService _toolService;
        private readonly IUnitOfWork _unitOfWork;
        public ToolsController(IToolService toolService, IUnitOfWork unitOfWork)
        {
            _toolService = toolService;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableTools(DateTime start, DateTime end)
        {
            var tools = await _toolService.GetAvailableToolsAsync(start, end);
            return Ok(tools);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetFilteredToolsAsync(ToolFilterRequest filterRequest)
        {
            var tools = await _toolService.FilterToolsAsync(filterRequest);
            return Ok(tools);
        }

        [HttpGet("details/{toolId}")]
        public async Task<IActionResult> GetToolDetails([FromRoute] int toolId)
        {
            var toolDetails = await _toolService.GetToolDetailsAsync(toolId);

            if (toolDetails == null)
                return NotFound();

            return Ok(toolDetails);
        }

        //[HttpPost("pickup")]
        //public async Task<IActionResult> PickUpTool([FromBody] int bookingId)
        //{
        //    var rented = "Rented";
        //    var success = await _toolService.SetToolAvailabilityAsync(bookingId, rented);
        //    if (!success)
        //        return BadRequest("Could not pick up tool.");
        //    await _unitOfWork.SaveChangesAsync();
        //    return Ok("Tool picked up successfully.");
        //}

        //[HttpPost("return")]
        //public async Task<IActionResult> ReturnTool([FromBody] int bookingId)
        //{
        //    var available = "Available";
        //    var success = await _toolService.SetToolAvailabilityAsync(bookingId, available);
        //    if (!success)
        //        return BadRequest("Could not pick up tool.");
        //    await _unitOfWork.SaveChangesAsync();
        //    return Ok("Tool returned up successfully.");
        //}

        // CRUD

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllTools()
        {
            var tools = await _toolService.GetAllToolsAsync();
            return Ok(tools);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateTool([FromBody] CreateToolRequest request)
        {
            var result = await _toolService.CreateToolAsync(request);
            await _unitOfWork.SaveChangesAsync();
            return Ok(new { message = result });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateTool(int id, [FromBody] UpdateToolRequest request)
        {
            var result = await _toolService.UpdateToolAsync(id, request);
            
            if (result == "tool not found")
                return NotFound(new { message = result });

            await _unitOfWork.SaveChangesAsync();
            return Ok(new { message = result });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTool(int id)
        {
            var success = await _toolService.DeleteToolAsync(id);
            
            if (!success)
                return NotFound(new { message = "Tool not found" });

            await _unitOfWork.SaveChangesAsync();
            return Ok(new { message = "Tool deleted successfully" });
        }

        [HttpGet("general-statistics")]
        public async Task<IActionResult> GetGeneralToolStatistics()
        {
            var statistics = await _toolService.GetGeneralToolStatisticsAsync();
            return Ok(statistics);
        }

        [HttpGet("usage-statistics")]
        public async Task<IActionResult> GetToolUsageStatistics()
        {
            var statistics = await _toolService.GetToolUsageStatisticsAsync();
            return Ok(statistics);
        }
    }
}
