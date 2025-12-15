using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Responses
{
    public class ToolUsageStatistics
    {
        public int ToolId { get; set; }
        public string ToolName { get; set; } = string.Empty;
        public int TotalBookings { get; set; }
        public int TotalDaysBooked { get; set; }
    }
}
