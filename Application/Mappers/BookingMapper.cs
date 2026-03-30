using Application.DTOs.Responses;
using Domain.Models;

namespace Application.Mappers;

public static class BookingMapper
{
    public static BookingsWithStatusResponse ToBookingsWithStatusResponse(Booking booking) => new()
    {
        BookingId = booking.Id,
        Status = booking.Status.ToString(),
        from = booking.StartDate,
        to = booking.EndDate,
        Tools = booking.Tools.Select(t => t.Name).ToArray()
    };
}