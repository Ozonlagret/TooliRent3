using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Responses
{
    public class BookingCompletionResult
    {
        public decimal LateFee { get; set; }
        public bool IsLate { get; set; }
    }
}
