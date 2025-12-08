using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;
using Application.DTOs.Requests;
using Application.Interfaces.Repository;

namespace Application.Services
{
    public class BookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IToolRepository _toolRepository;

        public BookingService(IBookingRepository bookingRepository, IToolRepository toolRepository)
        {
            _bookingRepository = bookingRepository;
            _toolRepository = toolRepository;
        }

        public async Task<(bool Success, string? Message, IEnumerable<int>? OverlappingToolIds)> BookToolsAsync(BookToolsRequest dto)
        {
            var tools = await _toolRepository.GetToolsByIdsAsync(dto.ToolIds);

            var unavailableToolIds = tools
                .Where(tool => tool.Availability != Domain.Enums.ToolAvailability.Available)
                .Select(tool => tool.Id)
                .ToList();

            if (unavailableToolIds.Any())
            {
                return (false, "Some tools are not available.", unavailableToolIds);
            }

            var overlappingTools = await _bookingRepository.GetOverlappingToolsAsync(dto.ToolIds, dto.StartDate, dto.EndDate);
            if (overlappingTools.Any())
            {
                return (false, "Some tools are already booked for the selected period.", overlappingTools);
            }

            var booking = new Booking
            {
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Tools = tools.ToList()
            };

            await _bookingRepository.AddAsync(booking);

            foreach (var tool in tools)
            {
                tool.Availability = Domain.Enums.ToolAvailability.Rented;
                await _toolRepository.UpdateToolAsync(tool);
            }

            return (true, "Booking successful.", null);
        }

        public async Task<IEnumerable<Booking>> GetBookingsAsync(string userId)
        {
            var bookings = await _bookingRepository.GetBookingsByUserIdAsync(userId);
            return bookings;
        }
    }
}
