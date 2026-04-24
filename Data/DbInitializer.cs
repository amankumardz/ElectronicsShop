using ElectronicsShop.Models;
using ElectronicsShop.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ElectronicsShop.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        await db.Database.MigrateAsync();

        string[] roles = ["Admin", "Customer"];
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        await EnsureUserAsync(userManager, "admin@shop.com", "Admin@123", "System Admin", "Admin");
        await EnsureUserAsync(userManager, "customer@shop.com", "Customer@123", "Demo Customer", "Customer");

        if (!await db.Categories.AnyAsync())
        {
            var categories = new List<Category>
            {
                new() { Name = "Home Appliances" },
                new() { Name = "Wiring Items" },
                new() { Name = "DJ Equipment" }
            };

            db.Categories.AddRange(categories);
            await db.SaveChangesAsync();
        }

        if (!await db.Products.AnyAsync())
        {
            var categoryMap = await db.Categories.ToDictionaryAsync(c => c.Name, c => c.Id);
            db.Products.AddRange(
                new Product { Name = "TV", CategoryId = categoryMap["Home Appliances"], Price = 30000, Description = "Smart LED TV" },
                new Product { Name = "Fan", CategoryId = categoryMap["Home Appliances"], Price = 1800, Description = "Ceiling fan" },
                new Product { Name = "Iron", CategoryId = categoryMap["Home Appliances"], Price = 1400, Description = "Dry iron" },
                new Product { Name = "LED Bulb", CategoryId = categoryMap["Wiring Items"], Price = 150, Description = "9W LED bulb" },
                new Product { Name = "Amplifier", CategoryId = categoryMap["DJ Equipment"], Price = 8500, Description = "DJ power amplifier" },
                new Product { Name = "Speaker", CategoryId = categoryMap["DJ Equipment"], Price = 4500, Description = "PA speaker" }
            );
            await db.SaveChangesAsync();
        }

        if (!await db.RepairRequests.AnyAsync())
        {
            var customer = await userManager.FindByEmailAsync("customer@shop.com");
            db.RepairRequests.AddRange(
                new RepairRequest
                {
                    CustomerId = customer?.Id,
                    CustomerName = customer?.FullName ?? "Walk-in Customer",
                    CustomerPhone = "1234567890",
                    DeviceName = "LED TV",
                    IssueDescription = "No display but sound is working",
                    Status = RepairStatus.Pending
                },
                new RepairRequest
                {
                    IsWalkIn = true,
                    CustomerName = "Rahul",
                    CustomerPhone = "5550101",
                    DeviceName = "Amplifier",
                    IssueDescription = "Power issue",
                    Status = RepairStatus.InProgress,
                    EstimatedCost = 1200
                }
            );
            await db.SaveChangesAsync();
        }
    }

    private static async Task EnsureUserAsync(UserManager<ApplicationUser> userManager, string email, string password, string fullName, string role)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = fullName,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Failed to create seed user {email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }

        if (!await userManager.IsInRoleAsync(user, role))
        {
            await userManager.AddToRoleAsync(user, role);
        }
    }
}
