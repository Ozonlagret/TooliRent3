using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Domain.Models;

namespace Application.Mappers;

public static class ToolCategoryMapper
{
    public static ToolCategoryResponse ToResponse(ToolCategory category) => new()
    {
        Id = category.Id,
        Name = category.Name,
        Description = category.Description,
        ToolCount = category.Tools?.Count ?? 0
    };

    public static ToolCategory ToEntity(CreateToolCategoryRequest request) => new()
    {
        Name = request.Name,
        Description = request.Description
    };

    public static void ApplyUpdate(ToolCategory category, UpdateToolCategoryRequest request)
    {
        category.Name = request.Name;
        category.Description = request.Description;
    }
}