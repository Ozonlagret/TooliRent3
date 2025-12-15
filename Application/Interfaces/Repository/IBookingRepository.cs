using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Models;

namespace Application.Interfaces.Repository
{
    public interface IBookingRepository
    {
        Task<Booking?> GetByIdAsync(int id);
        Task AddAsync(Booking booking);
        Task UpdateAsync(Booking booking);
        Task DeleteAsync(Booking booking);
        Task<IEnumerable<Booking>> GetBookingsByUserIdAsync(string userId);
    }
}
