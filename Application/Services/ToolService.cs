using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Interfaces.Repository;
using Domain.Enums;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ToolService
    {
        private IToolRepository _toolRepository;

        public ToolService(IToolRepository toolRepository)
        {
            _toolRepository=toolRepository;
        }

        public async Task<IEnumerable<Tool>> GetAvailableToolsAsync()
        {
            var tools = await _toolRepository.GetAvailableToolsAsync();
            
            return tools;
        }

        public async Task<IEnumerable<Tool?>> FilterToolsAsync(ToolFilterRequest filterDto)
        {
            var tools = await _toolRepository.FilterToolsAsync(filterDto);
            return tools;
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
