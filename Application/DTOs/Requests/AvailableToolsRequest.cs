using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Requests
{
    public class AvailableToolsRequest
    {
        public ToolAvailability Availability { get; set; }
    }
}
