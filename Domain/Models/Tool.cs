using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Tool
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal RentalPricePerDay { get; set; } 
        public string Condition { get; set; } = string.Empty;
        public int ToolCategoryId { get; set; }
        public ToolCategory ToolCategory { get; set; } = null!;
        public ToolStatus Status { get; set; }

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
