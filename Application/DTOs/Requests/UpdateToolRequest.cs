using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Requests
{
    public class UpdateToolRequest
    {
        [Required]
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal RentalPricePerDay { get; set; }
        public string? Condition { get; set; }
        public int CategoryId { get; set; }
        public string? ToolStatus { get; set; }
        public string? Availability { get; set; }
    }
}
