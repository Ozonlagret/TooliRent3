using System;
using Application.DTOs.Requests;
using Application.DTOs.Responses;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Service
{
    public interface IToolCategoryService
    {
        Task<IEnumerable<ToolCategoryResponse>> GetAllCategoriesAsync();
        Task<ToolCategoryResponse?> GetCategoryByIdAsync(int id);
        Task CreateCategoryAsync(CreateToolCategoryRequest request);
        Task<string> UpdateCategoryAsync(int id, UpdateToolCategoryRequest request);
        Task<bool> DeleteCategoryAsync(int id);
        Task<bool> CategoryExistsAsync(int id);
    }
}
