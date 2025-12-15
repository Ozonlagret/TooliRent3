using System;
using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Service
{
    public interface IBookingService
    {
        Task<(bool Success, string Message, IEnumerable<int> OverlappingToolIds)> BookToolsAsync(BookToolsRequest dto);
        Task<IEnumerable<Booking>> GetBookingsAsync(string userId);

    }
}
