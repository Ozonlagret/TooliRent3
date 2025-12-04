using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class TooliRentDbContext : DbContext 
    {
        public TooliRentDbContext(DbContextOptions<TooliRentDbContext> options)
       : base(options)
        {
        }
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
    }
}
