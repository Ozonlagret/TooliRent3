using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Requests
{
    public class ToolFilterRequest
    {
        public string? CategoryName { get; set; }
        public string? Name { get; set; }
        public ToolStatus? Status { get; set; }
        public ToolAvailability Availability { get; set; }
    }
}
