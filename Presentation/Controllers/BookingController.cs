using Microsoft.AspNetCore.Mvc;
using Application.Services;
using Application.Interfaces;
using Application.DTOs.Requests;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class BookingController
    {
        private readonly BookingService _bookingService;
        private readonly IUnitOfWork _unitOfWork;
        public BookingController(BookingService bookingService, IUnitOfWork unitOfWork)
        {
            _bookingService = bookingService;
            _unitOfWork = unitOfWork;
        }

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
        public async Task<IActionResult> CreateBooking(BookToolsRequest dto)
        {
            var result = await _bookingService.BookToolsAsync(dto);
            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Booking);
        }

        [HttpDelete("{bookingId}")]
        public async Task<IActionResult> CancelBooking(int bookingId)
        {
            var result = await _bookingService.CancelBookingAsync(bookingId);
            if (!result.Success)
                return BadRequest(result.Message);
            return Ok();
        }
    }
}
