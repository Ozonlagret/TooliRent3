using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class BookingController
    {
        [HttpGet]
        public async Task<IActionResult> GetBookings()
        {
            var bookings = await _bookingService.GetBookingsAsync();

        }

        [HttpGet]
        public async Task<IActionResult> GetBooking()
        {

        }

        [HttpPost] 
        public async Task<IActionResult> CreateBooking(BookToolsDto dto)
        {
            var result = await _bookingService.BookToolsAsync(dto);
            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Booking);
        }
    }
}
