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
        public int? ToolId { get; set; }
        public int? CategoryId { get; set; }
        public string? Status { get; set; }
        public string? Availability { get; set; }
    }
}
