using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [ApiController]
    // [controller] becomes 'tools' since the name of the class is toolsController
    [Route("[controller]")]
    public class ToolsController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAvailableTools()
        {
            return Ok();
        }

        [HttpGet("filter")]
        public IActionResult GetFilteredTools([FromQuery] string condition, [FromQuery] decimal maxRentalPricePerDay)
        {
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
