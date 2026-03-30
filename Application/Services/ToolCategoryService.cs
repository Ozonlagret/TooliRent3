using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Interfaces.Repository;
using Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces.Service;
using Application.Mappers;

namespace Application.Services
{
    public class ToolCategoryService : IToolCategoryService
    {
        private readonly IToolCategoryRepository _toolCategoryRepository;

        public ToolCategoryService(IToolCategoryRepository toolCategoryRepository)
        {
            _toolCategoryRepository = toolCategoryRepository;
        }

        public async Task<IEnumerable<ToolCategoryResponse>> GetAllCategoriesAsync()
        {
            var categories = await _toolCategoryRepository.GetAllAsync();
            return categories.Select(ToolCategoryMapper.ToResponse).ToList();
        }

        public async Task<ToolCategoryResponse?> GetCategoryByIdAsync(int id)
        {
            var category = await _toolCategoryRepository.GetByIdAsync(id);
            if (category == null)
                return null;

            return ToolCategoryMapper.ToResponse(category);
        }

        public async Task CreateCategoryAsync(CreateToolCategoryRequest request)
        {
            var category = ToolCategoryMapper.ToEntity(request);

            await _toolCategoryRepository.AddAsync(category);
        }

        public async Task<string> UpdateCategoryAsync(int id, UpdateToolCategoryRequest request)
        {
            var category = await _toolCategoryRepository.GetByIdAsync(id);
            if (category == null)
                return "category not found";

            ToolCategoryMapper.ApplyUpdate(category, request);

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
