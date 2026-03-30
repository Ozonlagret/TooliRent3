using Domain.Enums;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Data
{
    // helt AI genererat
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<TooliRentDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Apply migrations
            context.Database.Migrate();

            // Seed Roles
            SeedRoles(roleManager);

            // Seed Users
            SeedUsers(userManager);

            // Seed Tool Categories
            SeedToolCategories(context);

            // Seed Tools
            SeedTools(context);

            // Ensure status/availability combinations are valid for all tools
            NormalizeToolAvailabilityByStatus(context);

            // Seed Bookings
            SeedBookings(context, userManager);

            context.SaveChanges();
        }

        private static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Admin", "Member" };

            foreach (var role in roles)
            {
                if (!roleManager.RoleExistsAsync(role).Result)
                {
                    roleManager.CreateAsync(new IdentityRole(role)).Wait();
                }
            }
        }

        private static void SeedUsers(UserManager<ApplicationUser> userManager)
        {
            // Admin
            if (userManager.FindByNameAsync("admin").Result == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = "admin",
                    Email = "admin@toolirent.com",
                    EmailConfirmed = true,
                    IsActive = true
                };

                var result = userManager.CreateAsync(admin, "Admin123!").Result;
                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(admin, "Admin").Wait();
                }
            }

            // Regular Members
            var members = new[]
            {
                new { UserName = "john_doe", Email = "john@example.com", Password = "Member123!" },
                new { UserName = "jane_smith", Email = "jane@example.com", Password = "Member123!" },
                new { UserName = "bob_builder", Email = "bob@example.com", Password = "Member123!" }
            };

            foreach (var member in members)
            {
                if (userManager.FindByNameAsync(member.UserName).Result == null)
                {
                    var user = new ApplicationUser
                    {
                        UserName = member.UserName,
                        Email = member.Email,
                        EmailConfirmed = true,
                        IsActive = true
                    };

                    var result = userManager.CreateAsync(user, member.Password).Result;
                    if (result.Succeeded)
                    {
                        userManager.AddToRoleAsync(user, "Member").Wait();
                    }
                }
            }

            // Inactive User (for testing)
            if (userManager.FindByNameAsync("inactive_user").Result == null)
            {
                var inactiveUser = new ApplicationUser
                {
                    UserName = "inactive_user",
                    Email = "inactive@example.com",
                    EmailConfirmed = true,
                    IsActive = false
                };

                var result = userManager.CreateAsync(inactiveUser, "Inactive123!").Result;
                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(inactiveUser, "Member").Wait();
                }
            }
        }

        private static void SeedToolCategories(TooliRentDbContext context)
        {
            if (!context.ToolCategories.Any())
            {
                var categories = new[]
                {
                    new ToolCategory { Name = "Power Tools", Description = "Electric and battery-powered tools" },
                    new ToolCategory { Name = "Hand Tools", Description = "Manual tools for various tasks" },
                    new ToolCategory { Name = "Gardening Tools", Description = "Tools for lawn and garden maintenance" },
                    new ToolCategory { Name = "Ladders & Scaffolding", Description = "Equipment for working at height" },
                    new ToolCategory { Name = "Painting Equipment", Description = "Tools and equipment for painting projects" },
                    new ToolCategory { Name = "Cleaning Equipment", Description = "Tools for cleaning and maintenance" }
                };

                context.ToolCategories.AddRange(categories);
                context.SaveChanges();
            }
        }

        private static void SeedTools(TooliRentDbContext context)
        {
            if (!context.Tools.Any())
            {
                var categories = context.ToolCategories.ToList();

                var tools = new List<Tool>
                {
                    // Power Tools
                    new Tool
                    {
                        Name = "DeWalt Cordless Drill",
                        Description = "18V cordless drill with 2 batteries and charger",
                        RentalPricePerDay = 15.99m,
                        Condition = "Excellent",
                        ToolCategoryId = categories.First(c => c.Name == "Power Tools").Id,
                        Status = ToolStatus.Operational,
                        Availability = ToolAvailability.Available
                    },
                    new Tool
                    {
                        Name = "Makita Circular Saw",
                        Description = "7-1/4 inch circular saw with laser guide",
                        RentalPricePerDay = 20.00m,
                        Condition = "Good",
                        ToolCategoryId = categories.First(c => c.Name == "Power Tools").Id,
                        Status = ToolStatus.Operational,
                        Availability = ToolAvailability.Available
                    },
                    new Tool
                    {
                        Name = "Bosch Angle Grinder",
                        Description = "4-1/2 inch angle grinder, 7.5 amp",
                        RentalPricePerDay = 12.50m,
                        Condition = "Good",
                        ToolCategoryId = categories.First(c => c.Name == "Power Tools").Id,
                        Status = ToolStatus.Operational,
                        Availability = ToolAvailability.Available
                    },
                    new Tool
                    {
                        Name = "Milwaukee Impact Driver",
                        Description = "M18 brushless impact driver kit",
                        RentalPricePerDay = 18.00m,
                        Condition = "Excellent",
                        ToolCategoryId = categories.First(c => c.Name == "Power Tools").Id,
                        Status = ToolStatus.UnderMaintenance,
                        Availability = ToolAvailability.CurrentlyUnavailable
                    },

                    // Hand Tools
                    new Tool
                    {
                        Name = "Socket Set",
                        Description = "108-piece mechanics tool set with case",
                        RentalPricePerDay = 8.00m,
                        Condition = "Good",
                        ToolCategoryId = categories.First(c => c.Name == "Hand Tools").Id,
                        Status = ToolStatus.Operational,
                        Availability = ToolAvailability.Available
                    },
                    new Tool
                    {
                        Name = "Adjustable Wrench Set",
                        Description = "3-piece adjustable wrench set (6, 8, 10 inch)",
                        RentalPricePerDay = 5.00m,
                        Condition = "Fair",
                        ToolCategoryId = categories.First(c => c.Name == "Hand Tools").Id,
                        Status = ToolStatus.Operational,
                        Availability = ToolAvailability.Available
                    },

                    // Gardening Tools
                    new Tool
                    {
                        Name = "Lawn Mower",
                        Description = "Self-propelled gas lawn mower, 21 inch deck",
                        RentalPricePerDay = 35.00m,
                        Condition = "Good",
                        ToolCategoryId = categories.First(c => c.Name == "Gardening Tools").Id,
                        Status = ToolStatus.Operational,
                        Availability = ToolAvailability.Available
                    },
                    new Tool
                    {
                        Name = "Hedge Trimmer",
                        Description = "22-inch dual-action electric hedge trimmer",
                        RentalPricePerDay = 15.00m,
                        Condition = "Excellent",
                        ToolCategoryId = categories.First(c => c.Name == "Gardening Tools").Id,
                        Status = ToolStatus.Operational,
                        Availability = ToolAvailability.Available
                    },
                    new Tool
                    {
                        Name = "Leaf Blower",
                        Description = "Gas-powered backpack leaf blower",
                        RentalPricePerDay = 25.00m,
                        Condition = "Good",
                        ToolCategoryId = categories.First(c => c.Name == "Gardening Tools").Id,
                        Status = ToolStatus.Operational,
                        Availability = ToolAvailability.Rented
                    },

                    // Ladders & Scaffolding
                    new Tool
                    {
                        Name = "Extension Ladder",
                        Description = "24-foot aluminum extension ladder, 250 lb capacity",
                        RentalPricePerDay = 20.00m,
                        Condition = "Good",
                        ToolCategoryId = categories.First(c => c.Name == "Ladders & Scaffolding").Id,
                        Status = ToolStatus.Operational,
                        Availability = ToolAvailability.Available
                    },
                    new Tool
                    {
                        Name = "Step Ladder",
                        Description = "8-foot fiberglass step ladder",
                        RentalPricePerDay = 12.00m,
                        Condition = "Excellent",
                        ToolCategoryId = categories.First(c => c.Name == "Ladders & Scaffolding").Id,
                        Status = ToolStatus.Operational,
                        Availability = ToolAvailability.Available
                    },

                    // Painting Equipment
                    new Tool
                    {
                        Name = "Paint Sprayer",
                        Description = "Airless paint sprayer with hose and gun",
                        RentalPricePerDay = 40.00m,
                        Condition = "Good",
                        ToolCategoryId = categories.First(c => c.Name == "Painting Equipment").Id,
                        Status = ToolStatus.Operational,
                        Availability = ToolAvailability.Available
                    },
                    new Tool
                    {
                        Name = "Paint Roller Set",
                        Description = "Professional paint roller kit with extension pole",
                        RentalPricePerDay = 6.00m,
                        Condition = "Fair",
                        ToolCategoryId = categories.First(c => c.Name == "Painting Equipment").Id,
                        Status = ToolStatus.Operational,
                        Availability = ToolAvailability.Available
                    },

                    // Cleaning Equipment
                    new Tool
                    {
                        Name = "Pressure Washer",
                        Description = "3000 PSI gas pressure washer with hose and wand",
                        RentalPricePerDay = 45.00m,
                        Condition = "Excellent",
                        ToolCategoryId = categories.First(c => c.Name == "Cleaning Equipment").Id,
                        Status = ToolStatus.Operational,
                        Availability = ToolAvailability.Available
                    },
                    new Tool
                    {
                        Name = "Wet/Dry Vacuum",
                        Description = "16-gallon wet/dry shop vacuum",
                        RentalPricePerDay = 18.00m,
                        Condition = "Good",
                        ToolCategoryId = categories.First(c => c.Name == "Cleaning Equipment").Id,
                        Status = ToolStatus.Operational,
                        Availability = ToolAvailability.Available
                    }
                };

                context.Tools.AddRange(tools);
                context.SaveChanges();
            }
        }

        private static void NormalizeToolAvailabilityByStatus(TooliRentDbContext context)
        {
            var tools = context.Tools.ToList();
            var changed = false;

            foreach (var tool in tools)
            {
                var isOperational = tool.Status == ToolStatus.Operational;
                if (!isOperational && tool.Availability != ToolAvailability.CurrentlyUnavailable)
                {
                    tool.Availability = ToolAvailability.CurrentlyUnavailable;
                    changed = true;
                }
            }

            if (changed)
            {
                context.SaveChanges();
            }
        }

        private static void SeedBookings(TooliRentDbContext context, UserManager<ApplicationUser> userManager)
        {
            if (!context.Bookings.Any())
            {
                var john = userManager.FindByNameAsync("john_doe").Result;
                var jane = userManager.FindByNameAsync("jane_smith").Result;
                var bob = userManager.FindByNameAsync("bob_builder").Result;

                var leafBlower = context.Tools.FirstOrDefault(t => t.Name == "Leaf Blower");
                var pressureWasher = context.Tools.FirstOrDefault(t => t.Name == "Pressure Washer");
                var drill = context.Tools.FirstOrDefault(t => t.Name == "DeWalt Cordless Drill");

                var bookings = new List<Booking>();

                if (john != null && leafBlower != null)
                {
                    bookings.Add(new Booking
                    {
                        UserId = john.Id,
                        StartDate = DateTime.UtcNow.AddDays(-2),
                        EndDate = DateTime.UtcNow.AddDays(3),
                        ActualPickupDate = DateTime.UtcNow.AddDays(-2),
                        Status = BookingStatus.InProgress,
                        Tools = new List<Tool> { leafBlower }
                    });
                }

                if (jane != null && pressureWasher != null)
                {
                    pressureWasher.Availability = ToolAvailability.Reserved;
                    bookings.Add(new Booking
                    {
                        UserId = jane.Id,
                        StartDate = DateTime.UtcNow.AddDays(5),
                        EndDate = DateTime.UtcNow.AddDays(7),
                        Status = BookingStatus.Reserved,
                        Tools = new List<Tool> { pressureWasher }
                    });
                }

                if (bob != null && drill != null)
                {
                    bookings.Add(new Booking
                    {
                        UserId = bob.Id,
                        StartDate = DateTime.UtcNow.AddDays(-10),
                        EndDate = DateTime.UtcNow.AddDays(-5),
                        ActualPickupDate = DateTime.UtcNow.AddDays(-10),
                        ActualReturnDate = DateTime.UtcNow.AddDays(-5),
                        LateFee = 0,
                        Status = BookingStatus.Completed,
                        Tools = new List<Tool> { drill }
                    });
                }

                context.Bookings.AddRange(bookings);
                context.SaveChanges();
            }
        }
    }
}
