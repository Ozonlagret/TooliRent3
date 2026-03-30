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
        Task<(bool Success, string? Message, BookingsWithStatusResponse? Booking, IEnumerable<int>? OverlappingToolIds)> BookToolsAsync(BookToolsRequest dto, string userId);
        Task<IEnumerable<BookingsWithStatusResponse>> GetBookingsAsync(string userId);
        Task DeleteAllBookingsAsync();
        Task<ServiceResult> CancelBookingAsync(int bookingId, string userId, bool isAdmin);
        Task<string> MarkAsPickedUpAsync(int bookingId, string userId);
        Task<(string? Error, BookingCompletionResult? Result)> CompleteBookingAsync(int bookingId, string userId);
    }
}
