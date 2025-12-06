using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;
using Application.Interfaces.Repository;
using Infrastructure.Data;
using Microsoft.Identity.Client;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly TooliRentDbContext _dbContext;

        public RefreshTokenRepository(TooliRentDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(RefreshToken refreshToken)
        {
            await _dbContext.RefreshTokens.AddAsync(refreshToken);
        }

        public async Task<RefreshToken?> GetByUserIdAsync(string userId)
        {
            return await _dbContext.RefreshTokens
                .Where(rt => rt.UserId == userId && !rt.IsRevoked && rt.Expires > DateTime.UtcNow)
                .OrderByDescending(rt => rt.Created)
                .FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(RefreshToken refreshToken)
        {
            var existingToken = await _dbContext.RefreshTokens.FindAsync(refreshToken.Id);
            if (existingToken != null)
            {
                _dbContext.Entry(existingToken).CurrentValues.SetValues(refreshToken);
            }
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
