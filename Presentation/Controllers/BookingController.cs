using Microsoft.AspNetCore.Mvc;
using Application.Interfaces.Service;
using Application.Interfaces;
using Application.DTOs.Requests;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly IUnitOfWork _unitOfWork;
        
        public BookingController(IBookingService bookingService, IUnitOfWork unitOfWork)
        {
            _bookingService = bookingService;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("get-bookings")]
        public async Task<IActionResult> GetMyBookingsAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Unauthorized");
            var bookings = await _bookingService.GetBookingsAsync(userId);
            return Ok(bookings);
        }

        [HttpGet("{bookingId:int}")]
        public async Task<IActionResult> GetBooking(int bookingId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var bookings = await _bookingService.GetBookingsAsync(userId);
            var booking = bookings.FirstOrDefault(b => b.BookingId == bookingId);
            
            if (booking == null)
                return NotFound("Booking not found.");

            return Ok(booking);
        }

        [HttpPost] 
        public async Task<IActionResult> CreateBooking(BookToolsRequest dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            dto.UserId = userId;
            
            var result = await _bookingService.BookToolsAsync(dto);
            if (!result.Success)
                return BadRequest(new { message = result.Message, overlappingToolIds = result.OverlappingToolIds });

            await _unitOfWork.SaveChangesAsync();
            return Ok(result.Booking);
        }

        [HttpDelete("{bookingId:int}/cancel")]
        public async Task<IActionResult> CancelBooking(int bookingId)
        {
            var result = await _bookingService.CancelBookingAsync(bookingId);
            
            await _unitOfWork.SaveChangesAsync();
            return Ok(result);
        }

        [HttpPost("{bookingId:int}/pickup")]
        public async Task<IActionResult> PickupTools(int bookingId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();
            
            var result = await _bookingService.MarkAsPickedUpAsync(bookingId, userId);
            
            await _unitOfWork.SaveChangesAsync();
            return Ok(new { message = result });
        }

        [HttpPost("{bookingId:int}/return")]
        public async Task<IActionResult> ReturnTools(int bookingId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();
            
            var result = await _bookingService.CompleteBookingAsync(bookingId, userId);
            
            await _unitOfWork.SaveChangesAsync();

            return Ok(result);
        }
    }
}

