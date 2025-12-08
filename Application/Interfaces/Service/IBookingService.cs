using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Service
{
    public interface IBookingService
    {
        Task<(bool Success, string Message, IEnumerable<int> OverlappingToolIds)> BookToolsAsync(BookToolsDto dto);
        Task<IEnumerable<Booking>> GetBookingsAsync(string userId);
    }
}
