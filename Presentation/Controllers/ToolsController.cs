using Microsoft.AspNetCore.Mvc;
using Application.Services;
using Application.DTOs.Responses;
using Application.Interfaces;
using Application.DTOs.Requests;

namespace Presentation.Controllers
{
    [ApiController]
    // [controller] becomes 'tools' since the name of the class is toolsController
    [Route("[controller]")]
    public class ToolsController : ControllerBase
    {
        private readonly ToolService _toolService;
        private readonly IUnitOfWork _unitOfWork;
        public ToolsController(ToolService toolService, IUnitOfWork unitOfWork)
        {
            _toolService = toolService;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailableTools(DateTime start, DateTime end)
        {
            var tools = await _toolService.GetAvailableToolsAsync(start, end);
            await _unitOfWork.SaveChangesAsync();
            return Ok(tools);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetFilteredToolsAsync(ToolFilterRequest filterRequest)
        {
            var tools = _toolService.FilterToolsAsync(filterRequest);
            return Ok();
        }

        [HttpGet("details/[toolId]")]
        public IActionResult GetToolDetails([FromQuery] int toolId)
        {
            return Ok();
        }

        [HttpPost("pickup")]
        public IActionResult PickUpTool([FromBody] int bookingId)
        {
            // 1. Find the booking and tool
            // 2. Set tool status to CheckedOut
            // 3. Set booking.PickedUpAt = DateTime.UtcNow
            // 4. Save changes
            return Ok();
        }

        [HttpPost("return")]
        public IActionResult ReturnTool([FromBody] int bookingId)
        {
            // 1. Find the booking and tool
            // 2. Set booking.ReturnedAt = DateTime.UtcNow
            // 3. If ReturnedAt > booking.EndDate, set tool status to Late, else Available
            // 4. Save changes
            return Ok();
        }
    }
}
