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
        Task<IEnumerable<Tool>> GetAvailableToolsAsync(DateTime start, DateTime end);
        Task<IEnumerable<Tool>> FilterToolsAsync(DTOs.Requests.ToolFilterRequest filterRequest);
        Task<Tool?> GetToolDetailsAsync(int toolId);
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
