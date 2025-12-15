using Domain.Models;
using Domain.Enums;
using Domain.Filters;
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
                .Where(t => t.Availability == ToolAvailability.Available
                         && t.Status == ToolStatus.Operational)
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

        public async Task<IEnumerable<Tool>> FilterToolsAsync(ToolFilter filter)
        {
            var query = _dbContext.Tools.Include(t => t.ToolCategory).AsQueryable();

            if (filter.CategoryId.HasValue)
                query = query.Where(tool => tool.ToolCategory.Id == filter.CategoryId);

            if (filter.ToolId.HasValue)
                query = query.Where(tool => tool.Id == filter.ToolId);

            if (filter.Status != null)
                query = query.Where(tool => tool.Status == filter.Status);

            if (filter.Availability != null)
                query = query.Where(tool => tool.Availability == filter.Availability);

            return await query.ToListAsync();
        }

        public async Task UpdateToolAsync(Tool tool)
        {
            _dbContext.Tools.Update(tool);
        }

        public async Task<IEnumerable<Tool>> GetAllAsync()
        {
            return await _dbContext.Tools
                .Include(tool => tool.ToolCategory)
                .ToListAsync();
        }

        public async Task<Tool?> GetByIdAsync(int id)
        {
            return await _dbContext.Tools
                .Include(t => t.ToolCategory)
                .FirstOrDefaultAsync(tool => tool.Id == id);
        }

        public async Task<Tool> AddAsync(Tool tool)
        {
            await _dbContext.Tools.AddAsync(tool);
            return tool;
        }


        public async Task DeleteAsync(Tool tool)
        {
            _dbContext.Tools.Remove(tool);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _dbContext.Tools.AnyAsync(tool => tool.Id == id);
        }

        public async Task<IEnumerable<Tool>> GetAllToolsWithStatisticsAsync()
        {
            return await _dbContext.Tools
                .Include(t => t.Bookings)
                .ToListAsync();
        }
    }
}
