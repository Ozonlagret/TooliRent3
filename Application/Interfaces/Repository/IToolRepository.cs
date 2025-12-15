using Application.DTOs.Requests;
using Domain.Models;
using Domain.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repository
{
    public interface IToolRepository
    {
        Task<IEnumerable<Tool>> GetAvailableToolsAsync(DateTime start, DateTime end);
        Task<IEnumerable<Tool>> FilterToolsAsync(ToolFilter filter);
        Task<Tool?> GetToolByIdWithCategoryAsync(int toolId);
        Task<IEnumerable<Tool>> GetToolsByIdsAsync(IEnumerable<int> toolIds);
        Task<Tool> AddAsync(Tool tool);
        Task UpdateToolAsync(Tool tool);
        Task DeleteAsync(Tool tool);
        Task<bool> ExistsAsync(int toolId);
        Task<IEnumerable<Tool>> GetAllAsync();
        Task<IEnumerable<Tool>> GetAllToolsWithStatisticsAsync();
    }
}
