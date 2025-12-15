using Application.Interfaces.Repository;
using Domain.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class ToolCategoryRepository : IToolCategoryRepository
    {
        private readonly TooliRentDbContext _dbContext;

        public ToolCategoryRepository(TooliRentDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<ToolCategory>> GetAllAsync()
        {
            return await _dbContext.ToolCategories
                .Include(tc => tc.Tools)
                .ToListAsync();
        }

        public async Task<ToolCategory?> GetByIdAsync(int id)
        {
            return await _dbContext.ToolCategories
                .Include(tc => tc.Tools)
                .FirstOrDefaultAsync(tc => tc.Id == id);
        }

        public async Task<ToolCategory> AddAsync(ToolCategory category)
        {
            await _dbContext.ToolCategories.AddAsync(category);
            return category;
        }

        public async Task UpdateAsync(ToolCategory category)
        {
            _dbContext.ToolCategories.Update(category);
        }

        public async Task DeleteAsync(ToolCategory category)
        {
            _dbContext.ToolCategories.Remove(category);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _dbContext.ToolCategories.AnyAsync(tc => tc.Id == id);
        }
    }
}
