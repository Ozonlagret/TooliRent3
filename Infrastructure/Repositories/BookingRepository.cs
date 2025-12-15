using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces.Repository;
using Domain.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly TooliRentDbContext _dbContext;

        public BookingRepository(TooliRentDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Booking booking)
        {
            await _dbContext.Bookings.AddAsync(booking);
        }

        public async Task DeleteAsync(Booking booking)
        {
            _dbContext.Bookings.Remove(booking);
            await Task.CompletedTask;
        }

        public async Task<Booking?> GetByIdAsync(int id)
        {
            return await _dbContext.Bookings
                .Include(b => b.Tools)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<Booking>> GetBookingsByUserIdAsync(string userId)
        {
            return await _dbContext.Bookings
                .Where(b => b.UserId == userId)
                .Include(b => b.Tools)
                .ToListAsync();
        }

        public async Task UpdateAsync(Booking booking)
        {
            var existing = await _dbContext.Bookings.FindAsync(booking.Id);
            if (existing != null)
            {
                _dbContext.Entry(existing).CurrentValues.SetValues(booking);
            }
        }
    }
}
