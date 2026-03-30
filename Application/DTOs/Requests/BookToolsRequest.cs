using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace Application.DTOs.Requests
{
    public class BookToolsRequest
    {
        public int[] ToolIds { get; set; } = Array.Empty<int>();
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
