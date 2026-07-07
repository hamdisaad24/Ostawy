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

    public DbSet<EmailVerification> EmailVerifications { get; set; }
    public DbSet<PasswordResetOtp> PasswordResetOtps { get; set; }

    public DbSet<JobRequest> JobRequests { get; set; }
    public DbSet<JobBid> JobBids { get; set; }

    public DbSet<Category> Categories { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // إضافة تخصصات افتراضية في الداتا بيز
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "أعمال السباكة" },
            new Category { Id = 2, Name = "كهرباء منازل" },
            new Category { Id = 3, Name = "تكييف وتبريد" },
            new Category { Id = 4, Name = "نقاشة ودهانات" },
            new Category { Id = 5, Name = "نجارة وصيانة أثاث" }
        );
    }

}