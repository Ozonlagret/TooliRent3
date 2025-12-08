using Domain.Models;
using Domain.Enums;
using Application.DTOs.Requests;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Repository;

namespace Infrastructure.Repositories
{
    public class ToolRepository : IToolRepository
    {
        private readonly TooliRentDbContext _dbContext;
        public ToolRepository(TooliRentDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Tool>> GetAvailableToolsAsync(DateTime start, DateTime end)
        {
            return await _dbContext.Tools
                .Include(t => t.ToolCategory) 
                .Where(t => !t.Bookings.Any(b =>
                    b.Status != BookingStatus.Cancelled &&
                    b.EndDate > start &&
                    b.StartDate < end))
                    .ToListAsync();
        }

        public async Task<IEnumerable<Tool>> GetToolsByIdsAsync(IEnumerable<int> toolIds)
        {
            return await _dbContext.Tools
                .Where(tool => toolIds.Contains(tool.Id))
                .ToListAsync();
        }

        public async Task<Tool?> GetToolByIdWithCategoryAsync(int toolId)
        {
            return await _dbContext.Tools
                .Include(tool => tool.ToolCategory)
                .FirstOrDefaultAsync(tool => tool.Id == toolId);
        }

        public async Task<IEnumerable<Tool>> FilterToolsAsync(ToolFilterRequest filterDto)
        {
            var query = _dbContext.Tools.Include(t => t.ToolCategory).AsQueryable();

            if (!string.IsNullOrEmpty(filterDto.CategoryName))
                query = query.Where(tool => tool.ToolCategory.Name == filterDto.CategoryName);

            if (!string.IsNullOrEmpty(filterDto.Name))
                query = query.Where(tool => tool.Name == filterDto.Name);

            if (filterDto.Status != null)
                query = query.Where(tool => tool.Status == filterDto.Status);

            if (filterDto.Availability != null)
                query = query.Where(tool => tool.Availability == filterDto.Availability);

            return await query.ToListAsync();
        }

        public async Task UpdateToolAsync(Tool tool)
        {
            _dbContext.Tools.Update(tool);
        }
    }
}
