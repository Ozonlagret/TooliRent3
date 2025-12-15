using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Responses
{
    public class GeneralToolStatistics
    {
        public int TotalTools { get; set; }
        public int AvailableTools { get; set; }
        public int RentedTools { get; set; }
        public int UnderMaintenanceTools { get; set; }
    }
}
