using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Ostawy.Models;

namespace Ostawy.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<CraftsmanProfession>()
            .HasKey(cp => new { cp.CraftsmanId, cp.ProfessionId });
    }

    public DbSet<EmailVerification> EmailVerifications { get; set; }
    public DbSet<PasswordResetOtp> PasswordResetOtps { get; set; }
    public DbSet<Plan> Plans { get; set; }
    public DbSet<UserSubscription> UserSubscriptions { get; set; }
    public DbSet<Payment> Payments { get; set; }   
    public DbSet<Craftsman> Craftsmen { get; set; }
    public DbSet<CraftsmanProfession> Craftsmanprofessions { get; set; }
    public DbSet<Profession> Professions { get; set; }
}