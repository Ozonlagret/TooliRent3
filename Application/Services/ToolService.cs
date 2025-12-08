using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Interfaces.Repository;
using Domain.Enums;
using Domain.Models;
using Application.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Application.Services
{
    public class ToolService
    {
        private IToolRepository _toolRepository;

        public ToolService(IToolRepository toolRepository)
        {
            _toolRepository=toolRepository;
        }

        public async Task<IEnumerable<AvailableToolsResponse>> GetAvailableToolsAsync(DateTime start, DateTime end)
        {
            var tools = await _toolRepository.GetAvailableToolsAsync(start, end);

            var results = await q.Select(t => new AvailableToolsResponse
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                RentalPricePerDay = t.RentalPricePerDay,
                ToolCategoryId = t.ToolCategoryId,
                ToolCategoryName = t.ToolCategory != null ? t.ToolCategory.Name : null,
                Status = t.Status,
            }).ToListAsync();


            return results;
        }

        public async Task<IEnumerable<ToolDetailsResponse?>> FilterToolsAsync(ToolFilterRequest filterDto)
        {
            var tools = await _toolRepository.FilterToolsAsync(filterDto);

            var results = tools.Select(t => new ToolDetailsResponse
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                Status = t.Status.ToString(),
                Availability = t.Availability.ToString(),
                Category = t.ToolCategory != null ? t.ToolCategory.Name : null
            });
            return results;
        }

        public async Task<ToolDetailsResponse?> GetToolDetailsAsync(int toolId)
        {
            var tool = await _toolRepository.GetToolByIdWithCategoryAsync(toolId);
            return (new ToolDetailsResponse
            {
                Id = tool.Id,
                Name = tool?.Name,
                Description = tool?.Description,
                Status = tool.Status.ToString(),
                Availability = tool?.Availability.ToString(),
                Category = tool?.ToolCategory?.Name
            });
        }

        public async Task<bool> SetToolAvailabilityAsync(int toolId, ToolAvailability availability)
        {
            var tool = await _toolRepository.GetToolByIdWithCategoryAsync(toolId);
            if (tool == null)
                return false;

            tool.Availability = availability;
            await _toolRepository.UpdateToolAsync(tool);
            return true;
        }
    }
}
