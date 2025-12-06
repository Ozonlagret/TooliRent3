using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace Application.Services
{
    internal class BookingService
    {
        public async Task<(bool Success, string Message, IEnumerable<int> OverlappingToolIds)> BookToolsAsync(BookToolsDto dto)
        {
            var overlappingTools = await GetOverlappingToolsAsync(dto.ToolIds, dto.StartDate, dto.EndDate);
            if (overlappingTools.Any())
            {
                if (overlappingTools.Any())
                {
                    return (false, "Some tools are already booked for the selected period.", overlappingTools);
                }
            }
        }

        public async Task<IEnumerable<Booking>> GetBookingsAsync(string userId)
        {
            var bookings = await _bookingRepository.GetBookingsByUserIdAsync(userId);
            return bookings;
        }
    }
}
