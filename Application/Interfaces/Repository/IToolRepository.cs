using Application.DTOs.Requests;
using Domain.Models;
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
        Task<IEnumerable<Tool?>> FilterToolsAsync(ToolFilterRequest filterDto);
        Task<Tool?> GetToolByIdWithCategoryAsync(int toolId);
        Task UpdateToolAsync(Tool tool);
        Task<IEnumerable<Tool>> GetToolsByIdsAsync(IEnumerable<int> toolIds);
    }
}
