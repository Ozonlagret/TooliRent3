using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Interfaces.Repository;
using Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ToolCategoryService
    {
        private readonly IToolCategoryRepository _toolCategoryRepository;

        public ToolCategoryService(IToolCategoryRepository toolCategoryRepository)
        {
            _toolCategoryRepository = toolCategoryRepository;
        }

        public async Task<IEnumerable<ToolCategoryResponse>> GetAllCategoriesAsync()
        {
            var categories = await _toolCategoryRepository.GetAllAsync();
            return categories.Select(c => new ToolCategoryResponse
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                ToolCount = c.Tools?.Count ?? 0
            });
        }

        public async Task<ToolCategoryResponse?> GetCategoryByIdAsync(int id)
        {
            var category = await _toolCategoryRepository.GetByIdAsync(id);
            if (category == null)
                return null;

            return new ToolCategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ToolCount = category.Tools?.Count ?? 0
            };
        }

        public async Task CreateCategoryAsync(CreateToolCategoryRequest request)
        {
            var category = new ToolCategory
            {
                Name = request.Name,
                Description = request.Description
            };

            await _toolCategoryRepository.AddAsync(category);
        }

        public async Task<string> UpdateCategoryAsync(int id, UpdateToolCategoryRequest request)
        {
            var category = await _toolCategoryRepository.GetByIdAsync(id);
            if (category == null)
                return "category not found";

            category.Name = request.Name;
            category.Description = request.Description;

            await _toolCategoryRepository.UpdateAsync(category);

            return "update successful";
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _toolCategoryRepository.GetByIdAsync(id);
            if (category == null)
                return false;

            if (category.Tools != null && category.Tools.Any())
                return false;

            await _toolCategoryRepository.DeleteAsync(category);
            return true;
        }

        public async Task<bool> CategoryExistsAsync(int id)
        {
            return await _toolCategoryRepository.ExistsAsync(id);
        }
    }
}
