using Microsoft.AspNetCore.Identity;
using Ostawy.Models;

namespace API.SeedingData;

public static class AdminSeeder
{
    public static async Task SeedAsync(IServiceProvider service)
    {
        var userManager = service.GetRequiredService<UserManager<ApplicationUser>>();
        
        string adminEmail = "admin@ostawy.com";
        string password = Environment.GetEnvironmentVariable("ADMIN_PASSWORD")
              ?? throw new InvalidOperationException("ADMIN_PASSWORD is not set.");
        string roleName = "Admin";

        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var admin = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "Admin User",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(admin, password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Failed to create Admin: {errors}");
            }

            await userManager.AddToRoleAsync(admin, roleName);
        }
    }
}