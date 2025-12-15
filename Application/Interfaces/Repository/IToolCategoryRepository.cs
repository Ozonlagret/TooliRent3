using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces.Repository
{
    public interface IToolCategoryRepository
    {
        Task<IEnumerable<ToolCategory>> GetAllAsync();
        Task<ToolCategory?> GetByIdAsync(int id);
        Task<ToolCategory> AddAsync(ToolCategory category);
        Task UpdateAsync(ToolCategory category);
        Task DeleteAsync(ToolCategory category);
        Task<bool> ExistsAsync(int id);
    }
}
