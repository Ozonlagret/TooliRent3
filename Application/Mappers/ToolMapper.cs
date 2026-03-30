using Domain.Enums;
using Domain.Filters;
using Domain.Models;
using Application.DTOs.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Application.Mappers
{
    public class ToolMapper
    {
        public static ToolFilter ToToolFilter(ToolFilterRequest request)
        {
            ToolStatus? status = null;
            if (!string.IsNullOrEmpty(request.Status))
            {
                status = Enum.Parse<ToolStatus>(request.Status);
            }

            ToolAvailability? availability = null;
            if (!string.IsNullOrEmpty(request.Availability))
            {
                availability = Enum.Parse<ToolAvailability>(request.Availability);
            }

            return new ToolFilter (
                request.CategoryId,
                status,
                availability
            );
        }

        public static Tool ToTool(CreateToolRequest request)
        {
            var status = string.IsNullOrEmpty(request.ToolStatus)
                ? ToolStatus.Operational
                : Enum.Parse<ToolStatus>(request.ToolStatus);

            var requestedAvailability = string.IsNullOrEmpty(request.Availability)
                ? ToolAvailability.Available
                : Enum.Parse<ToolAvailability>(request.Availability);

            return new Tool
            {
                Name = request.Name ?? string.Empty,
                Description = request.Description ?? string.Empty,
                RentalPricePerDay = request.RentalPricePerDay,
                Condition = request.Condition ?? string.Empty,
                ToolCategoryId = request.CategoryId,
                Status = status,
                Availability = ResolveAvailability(status, requestedAvailability)
            };
        }

        public static void UpdateToolFromRequest(Tool tool, UpdateToolRequest request)
        {
            tool.Name = request.Name ?? tool.Name;
            tool.Description = request.Description ?? tool.Description;
            tool.RentalPricePerDay = request.RentalPricePerDay;
            tool.Condition = request.Condition ?? tool.Condition;
            tool.ToolCategoryId = request.CategoryId;

            if (!string.IsNullOrEmpty(request.ToolStatus))
                tool.Status = Enum.Parse<ToolStatus>(request.ToolStatus);

            var requestedAvailability = tool.Availability;
            if (!string.IsNullOrEmpty(request.Availability))
            {
                requestedAvailability = Enum.Parse<ToolAvailability>(request.Availability);
            }

            tool.Availability = ResolveAvailability(tool.Status, requestedAvailability);
        }

        private static ToolAvailability ResolveAvailability(ToolStatus status, ToolAvailability requestedAvailability)
        {
            return status == ToolStatus.Operational ? requestedAvailability : ToolAvailability.CurrentlyUnavailable;
        }
    }
}
