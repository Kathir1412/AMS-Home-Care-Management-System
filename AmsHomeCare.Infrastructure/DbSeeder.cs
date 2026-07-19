using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AmsHomeCare.Core.Entities;
using AmsHomeCare.Core.Enums;
using AmsHomeCare.Infrastructure.Identity;

namespace AmsHomeCare.Infrastructure.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Apply migrations automatically if needed
            if (context.Database.GetPendingMigrations().Any())
            {
                await context.Database.MigrateAsync();
            }

            // Seed Roles
            string[] roles = { "Admin", "Supervisor" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Seed Admin User
            var adminEmail = "admin@amshomecare.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "AMS Admin",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(adminUser, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // Seed Supervisor User
            var supervisorEmail = "supervisor@amshomecare.com";
            var supervisorUser = await userManager.FindByEmailAsync(supervisorEmail);
            if (supervisorUser == null)
            {
                supervisorUser = new ApplicationUser
                {
                    UserName = supervisorEmail,
                    Email = supervisorEmail,
                    FullName = "AMS Supervisor",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(supervisorUser, "Supervisor@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(supervisorUser, "Supervisor");
                }
            }

            // Seed Settings
            if (!context.Settings.Any())
            {
                context.Settings.AddRange(
                    new Setting { Key = "CompanyName", Value = "AMS Home Care" },
                    new Setting { Key = "CompanyAddress", Value = "123 Healthcare Way, Suite 100, Chennai" },
                    new Setting { Key = "CompanyPhone", Value = "+91 98765 43210" },
                    new Setting { Key = "CompanyEmail", Value = "info@amshomecare.com" },
                    new Setting { Key = "LogoPath", Value = "/images/logo.png" }
                );
                await context.SaveChangesAsync();
            }

            // Seed Default Shifts
            if (!context.Shifts.Any())
            {
                context.Shifts.AddRange(
                    new Shift { ShiftName = "Morning Shift", StartTime = new TimeSpan(6, 0, 0), EndTime = new TimeSpan(14, 0, 0), WeeklyOffDay = DayOfWeek.Sunday, IsCustom = false },
                    new Shift { ShiftName = "Evening Shift", StartTime = new TimeSpan(14, 0, 0), EndTime = new TimeSpan(22, 0, 0), WeeklyOffDay = DayOfWeek.Sunday, IsCustom = false },
                    new Shift { ShiftName = "Night Shift", StartTime = new TimeSpan(22, 0, 0), EndTime = new TimeSpan(6, 0, 0), WeeklyOffDay = DayOfWeek.Sunday, IsCustom = false }
                );
                await context.SaveChangesAsync();
            }

            // Seed Holidays
            if (!context.Holidays.Any())
            {
                var currentYear = DateTime.Now.Year;
                context.Holidays.AddRange(
                    new Holiday { Name = "New Year's Day", Date = new DateTime(currentYear, 1, 1) },
                    new Holiday { Name = "Pongal", Date = new DateTime(currentYear, 1, 15) },
                    new Holiday { Name = "Republic Day", Date = new DateTime(currentYear, 1, 26) },
                    new Holiday { Name = "May Day", Date = new DateTime(currentYear, 5, 1) },
                    new Holiday { Name = "Independence Day", Date = new DateTime(currentYear, 8, 15) },
                    new Holiday { Name = "Gandhi Jayanti", Date = new DateTime(currentYear, 10, 2) },
                    new Holiday { Name = "Christmas Day", Date = new DateTime(currentYear, 12, 25) }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}
