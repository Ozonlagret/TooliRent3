using Domain.Enums;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? ActualPickupDate { get; set; }   
        public DateTime? ActualReturnDate { get; set; }    
        public decimal LateFee { get; set; } = 0;        
        public BookingStatus Status { get; set; }
        
        public ICollection<Tool> Tools { get; set; } = new List<Tool>();
    }
}
