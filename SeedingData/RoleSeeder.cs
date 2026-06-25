using Microsoft.AspNetCore.Identity;

namespace Ostawy.SeedingData;

public static class RoleSeeder
{
    private static readonly string[] Roles = { "Admin", "Client", "CraftMan" };

    public static async Task SeedRolesAsync(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        foreach (var role in Roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
        }
    }
}
