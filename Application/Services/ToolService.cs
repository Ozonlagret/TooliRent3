using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Interfaces.Repository;
using Domain.Enums;
using Domain.Models;
using Application.Interfaces.Service;
using Application.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Application.Services
{
    public class ToolService : IToolService
    {
        private IToolRepository _toolRepository;
        private IBookingRepository _bookingRepository;

        public ToolService(IToolRepository toolRepository, IBookingRepository bookingRepository)
        {
            _toolRepository = toolRepository;
            _bookingRepository = bookingRepository;
        }

        //lista alla tillgängliga verktyg
        public async Task<IEnumerable<AvailableToolsResponse>> GetAvailableToolsAsync(DateTime start, DateTime end)
        {
            var tools = await _toolRepository.GetAvailableToolsAsync(start, end);    

            return tools.Select(ToolResponseMapper.ToAvailableToolsResponse).ToList();
        }

        // Filtrera verktyg efter kategori, status, tillgänglighet
        public async Task<IEnumerable<ToolResponse>> FilterToolsAsync(ToolFilterRequest filterDto)
        {
            var filter = ToolMapper.ToToolFilter(filterDto);

            var tools = await _toolRepository.FilterToolsAsync(filter);

            return tools.Select(ToolResponseMapper.ToToolResponse).ToList();
        }

        // Visa detaljerad information om specifika verktyg
        public async Task<ToolResponse?> GetToolDetailsAsync(int toolId)
        {
            var tool = await _toolRepository.GetToolByIdWithCategoryAsync(toolId);

            if (tool == null)
                return null;

            return ToolResponseMapper.ToToolResponse(tool);
        }

        // Markera att verktyget har hämtats ut/återlämnats
        public async Task<bool> SetToolAvailabilityAsync(int bookingId, string availability)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);

	        if (booking == null || booking.Tools == null || !booking.Tools.Any())
	           return false;

		    foreach (var tool in booking.Tools)
	        {
	           tool.Availability = Enum.Parse<ToolAvailability>(availability);
	           await _toolRepository.UpdateToolAsync(tool);
            }

	        return true;
        }

        // CRUD - Read All
        public async Task<IEnumerable<ToolResponse>> GetAllToolsAsync()
        {
            var tools = await _toolRepository.GetAllAsync();

            return tools.Select(ToolResponseMapper.ToToolResponse).ToList();
        }

        // CRUD - Create
        public async Task<string> CreateToolAsync(CreateToolRequest request)
        {
            var tool = ToolMapper.ToTool(request);
            var createdTool = await _toolRepository.AddAsync(tool);
            var toolWithCategory = await _toolRepository.GetToolByIdWithCategoryAsync(createdTool.Id);

            return "creation successful";
        }

        // CRUD - Update
        public async Task<string> UpdateToolAsync(int id, UpdateToolRequest request)
        {
            var tool = await _toolRepository.GetToolByIdWithCategoryAsync(id);
            if (tool == null)
                return "tool not found";

            ToolMapper.UpdateToolFromRequest(tool, request);

            await _toolRepository.UpdateToolAsync(tool);

            return "update successful";
        }

        // CRUD - Delete
        public async Task<bool> DeleteToolAsync(int id)
        {
            var tool = await _toolRepository.GetToolByIdWithCategoryAsync(id);
            if (tool == null)
                return false;

            await _toolRepository.DeleteAsync(tool);
            return true;
        }

        public async Task<bool> ToolExistsAsync(int id)
        {
            return await _toolRepository.ExistsAsync(id);
        }

        public async Task<GeneralToolStatistics> GetGeneralToolStatisticsAsync()
        {
            var tools = await _toolRepository.GetAllAsync();

            var statistics = new GeneralToolStatistics
            {
                TotalTools = tools.Count(),
                AvailableTools = tools.Count(t => t.Availability == ToolAvailability.Available),
                RentedTools = tools.Count(t => t.Availability == ToolAvailability.Rented),
                UnderMaintenanceTools = tools.Count(t => t.Status == ToolStatus.UnderMaintenance)
            };

            return statistics;
        }

        public async Task<IEnumerable<ToolUsageStatistics>> GetToolUsageStatisticsAsync()
        {
            var tools = await _toolRepository.GetAllToolsWithStatisticsAsync();

            var statistics = tools.Select(t => new ToolUsageStatistics
            {
                ToolId = t.Id,
                ToolName = t.Name,
                TotalBookings = t.Bookings.Count(b => b.Status != BookingStatus.Cancelled),
                TotalDaysBooked = t.Bookings
                    .Where(b => b.Status != BookingStatus.Cancelled)
                    .Sum(b => (b.EndDate - b.StartDate).Days)
            })
            .OrderByDescending(s => s.TotalBookings)
            .ToList();

            return statistics;
        }
    }
}
