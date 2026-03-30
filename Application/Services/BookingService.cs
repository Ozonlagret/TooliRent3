using Domain.Enums;
using Application.Interfaces.Service;
using Domain.Models;
using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Interfaces.Repository;

namespace Application.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IToolRepository _toolRepository;

        public BookingService(IBookingRepository bookingRepository, IToolRepository toolRepository)
        {
            _bookingRepository = bookingRepository;
            _toolRepository = toolRepository;
        }

        public async Task<(bool Success, string? Message, BookingsWithStatusResponse? Booking, IEnumerable<int>? OverlappingToolIds)> BookToolsAsync(BookToolsRequest dto, string userId)
        {
            if ((dto.EndDate.Date - dto.StartDate.Date).TotalDays < 1)
            {
                return (false, "Booking must span at least 1 full day.", null, null);
            }

            var tools = await _toolRepository.GetToolsByIdsAsync(dto.ToolIds);

            var unavailableTools = tools
                .Where(tool => tool.Availability != ToolAvailability.Available 
                            || tool.Status != ToolStatus.Operational)
                .Select(tool => tool.Id)
                .ToList();

            if (unavailableTools.Any())
            {
                return (false, "Some tools are not available for booking.", null, unavailableTools);
            }

            var booking = new Booking
            {
                UserId = userId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = BookingStatus.Reserved,
                Tools = tools.ToList()
            };

            await _bookingRepository.AddAsync(booking);

            foreach (var tool in tools)
            {
                tool.Availability = ToolAvailability.Reserved;
                await _toolRepository.UpdateToolAsync(tool);
            }

            var response = new BookingsWithStatusResponse
            {
                BookingId = booking.Id,
                Status = "Reserved",
                from = booking.StartDate,
                to = booking.EndDate,
                Tools = tools.Select(t => t.Name).ToArray()
            };

            return (true, "Booking successful.", response, null);
        }

        public async Task<IEnumerable<BookingsWithStatusResponse>> GetBookingsAsync(string userId)
        {
            var bookings = await _bookingRepository.GetBookingsByUserIdAsync(userId);

            var bookingsWithStatus = bookings.Select(b => new BookingsWithStatusResponse
            {
                BookingId = b.Id,
                Status = b.Status.ToString(),
                from = b.StartDate,
                to = b.EndDate,
                Tools = b.Tools.Select(t => t.Name).ToArray()
            });

            return bookingsWithStatus;
        }

        public async Task DeleteAllBookingsAsync()
        {
            await _bookingRepository.DeleteAllAsync();
        }

        public async Task<ServiceResult> CancelBookingAsync(int bookingId, string userId, bool isAdmin)
        {
            if (bookingId <= 0)
            {
                return ServiceResult.Validation("Invalid booking id.");
            }

            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            if (booking == null)
            {
                return ServiceResult.NotFound("Booking not found.");
            }

            if (!isAdmin && booking.UserId != userId)
            {
                return ServiceResult.Forbidden("You cannot cancel another user's booking.");
            }

            if (booking.Status is BookingStatus.Completed or BookingStatus.Cancelled)
            {
                return ServiceResult.Conflict("Booking cannot be cancelled in its current status.");
            }
            
            try
            {
                foreach (var tool in booking.Tools)
                {
                    if (tool.Status == ToolStatus.Operational)
                    {
                        tool.Availability = ToolAvailability.Available;
                    }
                    else
                    {
                        tool.Availability = ToolAvailability.Reserved;
                    }
                    await _toolRepository.UpdateToolAsync(tool);
                }

                booking.Status = BookingStatus.Cancelled;
                await _bookingRepository.UpdateAsync(booking);

                return ServiceResult.Success("Booking cancelled successfully.");
            }
            catch
            {
                return ServiceResult.Error("An unexpected error occurred while cancelling booking.");
            }
            
        }

        public async Task<string> MarkAsPickedUpAsync(int bookingId, string userId)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            
            if (booking == null)
                return "Booking not found.";
            
            if (booking.UserId != userId)
                return "Unauthorized access to this booking.";
            
            if (booking.Status != BookingStatus.Reserved)
                return $"Cannot pick up booking with status: {booking.Status}";
            
            if (DateTime.UtcNow.Date < booking.StartDate.Date)
                return "Cannot pick up before the booking start date.";
            
            booking.ActualPickupDate = DateTime.UtcNow;
            booking.Status = BookingStatus.InProgress;
            await _bookingRepository.UpdateAsync(booking);
            
            foreach (var tool in booking.Tools)
            {
                tool.Availability = ToolAvailability.Rented;
                await _toolRepository.UpdateToolAsync(tool);
            }
            
            return "Tools picked up successfully.";
        }

        public async Task<(string? Error, BookingCompletionResult? Result)> CompleteBookingAsync(int bookingId, string userId)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            
            if (booking == null)
                return ("Booking not found.", null);
            
            if (booking.UserId != userId)
                return ("Unauthorized access to this booking.", null);
            
            if (booking.Status != BookingStatus.InProgress)
                return ($"Cannot return booking with status: {booking.Status}", null);
            
            var returnDate = DateTime.UtcNow;
            var lateFee = CalculateLateFee(booking.EndDate, returnDate, booking.Tools);
            
            booking.ActualReturnDate = returnDate;
            booking.LateFee = lateFee;
            booking.Status = BookingStatus.Completed;
            await _bookingRepository.UpdateAsync(booking);
            
            foreach (var tool in booking.Tools)
            {
                if (tool.Status == ToolStatus.Operational)
                {
                    tool.Availability = ToolAvailability.Available;
                }
                else
                {
                    tool.Availability = ToolAvailability.CurrentlyUnavailable;
                }
                await _toolRepository.UpdateToolAsync(tool);
            }
            
            return (null, new BookingCompletionResult
            {
                LateFee = lateFee,
                IsLate = lateFee > 0
            });
        }

        private decimal CalculateLateFee(DateTime expectedReturn, DateTime actualReturn, IEnumerable<Tool> tools)
        {
            if (actualReturn.Date <= expectedReturn.Date)
                return 0;
            
            var lateDays = (actualReturn.Date - expectedReturn.Date).Days;
            var dailyRentalCost = tools.Sum(t => t.RentalPricePerDay);
            
            return lateDays * dailyRentalCost * 0.5m;
        }
    }
}
