using System;
using Domain.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Filters
{
    // for use with mappers
    public class ToolFilter
    {
        public int? ToolId { get; set; }
        public int? CategoryId { get; set; }
        public ToolStatus? Status { get; set; }
        public ToolAvailability? Availability { get; set; }

        public ToolFilter(int? toolId, int? categoryId, ToolStatus? status, ToolAvailability? availability)
        {
            ToolId = toolId;
            CategoryId = categoryId;
            Status = status;
            Availability = availability;
        }
    }
}
