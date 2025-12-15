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
                request.ToolId,
                request.CategoryId,
                status,
                availability
            );
        }

        public static Tool ToTool(CreateToolRequest request)
        {
            return new Tool
            {
                Name = request.Name,
                Description = request.Description,
                RentalPricePerDay = request.RentalPricePerDay,
                Condition = request.Condition,
                ToolCategoryId = request.CategoryId,
                Status = string.IsNullOrEmpty(request.ToolStatus)
                    ? ToolStatus.Operational
                    : Enum.Parse<ToolStatus>(request.ToolStatus),
                Availability = string.IsNullOrEmpty(request.Availability)
                    ? ToolAvailability.Available
                    : Enum.Parse<ToolAvailability>(request.Availability)
            };
        }

        public static void UpdateToolFromRequest(Tool tool, UpdateToolRequest request)
        {
            tool.Name = request.Name;
            tool.Description = request.Description;
            tool.RentalPricePerDay = request.RentalPricePerDay;
            tool.Condition = request.Condition;
            tool.ToolCategoryId = request.CategoryId;

            if (!string.IsNullOrEmpty(request.ToolStatus))
                tool.Status = Enum.Parse<ToolStatus>(request.ToolStatus);

            if (!string.IsNullOrEmpty(request.Availability))
            {
                var newAvailability = Enum.Parse<ToolAvailability>(request.Availability);
                
                if (newAvailability == ToolAvailability.Available && tool.Status != ToolStatus.Operational)
                {
                    tool.Availability = ToolAvailability.CurrentlyUnavailable;
                }
                else
                {
                    tool.Availability = newAvailability;
                }
            }
        }
    }
}
