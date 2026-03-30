using Application.DTOs.Responses;
using Domain.Models;

namespace Application.Mappers;

public static class ToolResponseMapper
{
    public static ToolResponse ToToolResponse(Tool tool) => new()
    {
        Id = tool.Id,
        Name = tool.Name,
        Description = tool.Description,
        RentalPricePerDay = tool.RentalPricePerDay,
        Condition = tool.Condition,
        Status = tool.Status.ToString(),
        Availability = tool.Availability.ToString(),
        Category = tool.ToolCategory?.Name
    };

    public static AvailableToolsResponse ToAvailableToolsResponse(Tool tool) => new()
    {
        Id = tool.Id,
        Name = tool.Name,
        Description = tool.Description,
        RentalPricePerDay = tool.RentalPricePerDay,
        ToolCategoryId = tool.ToolCategoryId,
        ToolCategoryName = tool.ToolCategory?.Name,
        Status = tool.Status.ToString()
    };
}