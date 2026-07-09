using Microsoft.EntityFrameworkCore;
using Ostawy.Data;
using Ostawy.Models;

namespace Ostawy.SeedingData;

public static class RequestSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (await context.JobRequests.AnyAsync())
            return;

        var users = await context.Users.Take(3).ToListAsync();
        if (users.Count == 0) return;

        var requests = new List<JobRequest>
        {
            new()
            {
                ClientId = users[0].Id.ToString(),
                CategoryId = 1,
                Description = "تسريب مياه من المواسير أسفل المغسلة في الحمام، الموية بتجري حتى لو مقفلة.",
                EstimatedPrice = 300,
                Status = "Open",
                CreatedAt = DateTime.Now.AddDays(-5)
            },
            new()
            {
                ClientId = users[1 % users.Count].Id.ToString(),
                CategoryId = 2,
                Description = "تركيب ٥ مراوح شفط في شقة جديدة، مع توصيل الكهرباء لكل مروحة.",
                EstimatedPrice = 1500,
                Status = "Open",
                CreatedAt = DateTime.Now.AddDays(-3)
            },
            new()
            {
                ClientId = users[2 % users.Count].Id.ToString(),
                CategoryId = 3,
                Description = "تكييف سبليت ٣ حصان مش بيعمل تبريد كويس، ممكن يحتاج فريون أو صيانة.",
                EstimatedPrice = 500,
                Status = "Accepted",
                CreatedAt = DateTime.Now.AddDays(-2)
            },
            new()
            {
                ClientId = users[0].Id.ToString(),
                CategoryId = 1,
                Description = "سخان مياه بينقط موية من تحت، عايز حد يصلحه قبل ما يبوظ بالكامل.",
                EstimatedPrice = 250,
                Status = "Closed",
                CreatedAt = DateTime.Now.AddDays(-7)
            }
        };

        context.JobRequests.AddRange(requests);
        await context.SaveChangesAsync();
    }
}
