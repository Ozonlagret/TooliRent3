using Microsoft.AspNetCore.Mvc;
using Application.Interfaces.Service;
using Application.Interfaces;
using Application.DTOs.Requests;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("member/bookings")]
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

        [HttpGet]
        public async Task<IActionResult> GetMyBookingsAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Unauthorized." });
            var bookings = await _bookingService.GetBookingsAsync(userId);
            return Ok(bookings);
        }

        [HttpGet("{bookingId:int}")]
        public async Task<IActionResult> GetBooking(int bookingId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Unauthorized." });

            var bookings = await _bookingService.GetBookingsAsync(userId);
            var booking = bookings.FirstOrDefault(b => b.BookingId == bookingId);
            
            if (booking == null)
                return NotFound(new { message = "Booking not found." });

            return Ok(booking);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBooking(BookToolsRequest dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) 
                return Unauthorized(new { message = "Unauthorized." });
            
            var result = await _bookingService.BookToolsAsync(dto, userId);
            if (!result.Success)
                return BadRequest(new { message = result.Message, overlappingToolIds = result.OverlappingToolIds });

            await _unitOfWork.SaveChangesAsync();
            return Ok(result.Booking);
        }

        [HttpDelete("{bookingId:int}/cancel")]
        public async Task<IActionResult> CancelBooking(int bookingId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Unauthorized." });

            var isAdmin = User.IsInRole("Admin");
            var result = await _bookingService.CancelBookingAsync(bookingId, userId, isAdmin);

            if (result.Code == "ValidationError")
                return BadRequest(new { message = result.Message });

            if (result.Code == "NotFound")
                return NotFound(new { message = result.Message });

            if (result.Code == "Forbidden")
                return StatusCode(StatusCodes.Status403Forbidden, new { message = result.Message });

            if (result.Code == "Conflict")
                return Conflict(new { message = result.Message });

            if (result.Code == "ServerError")
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = result.Message });
            
            await _unitOfWork.SaveChangesAsync();
            return Ok(new { message = result.Message });
        }

        [HttpPost("{bookingId:int}/pickup")]
        public async Task<IActionResult> PickUpTools(int bookingId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Unauthorized." });
            
            var result = await _bookingService.MarkAsPickedUpAsync(bookingId, userId);

            if (result == "Booking not found.")
                return NotFound(new { message = result });

            if (result == "Unauthorized access to this booking.")
                return StatusCode(StatusCodes.Status403Forbidden, new { message = result });

            if (result != "Tools picked up successfully.")
                return BadRequest(new { message = result });
            
            await _unitOfWork.SaveChangesAsync();
            return Ok(new { message = result });
        }

        [HttpPost("{bookingId:int}/return")]
        public async Task<IActionResult> ReturnTools(int bookingId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Unauthorized." });
            
            var result = await _bookingService.CompleteBookingAsync(bookingId, userId);

            if (!string.IsNullOrEmpty(result.Error))
            {
                if (result.Error == "Booking not found.")
                    return NotFound(new { message = result.Error });

                if (result.Error == "Unauthorized access to this booking.")
                    return StatusCode(StatusCodes.Status403Forbidden, new { message = result.Error });

                return BadRequest(new { message = result.Error });
            }
            
            await _unitOfWork.SaveChangesAsync();

            return Ok(result.Result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("~/admin/bookings/breakGlassInCaseOfEmergency/delete all bookings")]
        public async Task<IActionResult> ResetBookings()
        {
            await _bookingService.DeleteAllBookingsAsync();
            await _unitOfWork.SaveChangesAsync();

            return Ok(new
            {
                message = "All bookings have been deleted",
            });
        }
    }
}

