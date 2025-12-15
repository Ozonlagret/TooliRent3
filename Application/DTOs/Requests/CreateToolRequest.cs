using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Requests
{
    public class CreateToolRequest
    {
        [Required]
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal RentalPricePerDay { get; set; }
        public string Condition { get; set; } = "New";
        public int CategoryId { get; set; }
        public string ToolStatus { get; set; } = "Operational";
        public string Availability { get; set; } = "Available";
    }
}
