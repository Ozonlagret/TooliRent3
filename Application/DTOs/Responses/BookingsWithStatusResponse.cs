using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Responses
{
    public class BookingsWithStatusResponse
    {
        public int BookingId { get; set; }
        public string Status { get; set; } = string.Empty;

        public DateTime from { get; set; }
        public DateTime to { get; set; }

        public string[] Tools { get; set; } = Array.Empty<string>();
    }
}
