using Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class TooliRentDbContext : IdentityDbContext<ApplicationUser> 
    {
        public TooliRentDbContext(DbContextOptions<TooliRentDbContext> options)
       : base(options)
        {
        }
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
        public DbSet<Booking> Bookings { get; set; } = null!;
        public DbSet<Tool> Tools { get; set; } = null!;
        public DbSet<ToolCategory> ToolCategories { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RefreshToken>()
                .HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<Booking>()
                .HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<Tool>()
                .HasOne(t => t.ToolCategory)
                .WithMany(tc => tc.Tools)
                .HasForeignKey(t => t.ToolCategoryId)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<Booking>()
                .HasMany(b => b.Tools)
                .WithMany(t => t.Bookings)
                .UsingEntity(j => j.ToTable("BookingTools"));
        }
    }
}
