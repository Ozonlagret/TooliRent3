using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Responses
{
    public class AvailableToolsResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal RentalPricePerDay { get; set; }
        public int ToolCategoryId { get; set; }
        public string? ToolCategoryName { get; set; }
        public string? Status { get; set; }
    }
}
