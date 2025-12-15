using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Service
{
    public interface IToolService
    {
        Task<IEnumerable<AvailableToolsResponse>> GetAvailableToolsAsync(DateTime start, DateTime end);
        Task<IEnumerable<ToolResponse>> FilterToolsAsync(ToolFilterRequest filterRequest);
        Task<ToolResponse?> GetToolDetailsAsync(int toolId);
        Task<bool> SetToolAvailabilityAsync(int bookingId, string status);
        Task<IEnumerable<ToolResponse>> GetAllToolsAsync();
        Task<string> CreateToolAsync(CreateToolRequest request);
        Task<string> UpdateToolAsync(int id, UpdateToolRequest request);
        Task<bool> DeleteToolAsync(int id);
        Task<bool> ToolExistsAsync(int id);
        Task<GeneralToolStatistics> GetGeneralToolStatisticsAsync();
        Task<IEnumerable<ToolUsageStatistics>> GetToolUsageStatisticsAsync();
    }
}
