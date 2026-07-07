using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Ostawy.Models;
using System;
using System.Reflection.Emit;

namespace Ostawy.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // إضافة تخصصات افتراضية في الداتا بيز للنظام الموحد
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "أعمال السباكة" },
                new Category { Id = 2, Name = "كهرباء منازل" },
                new Category { Id = 3, Name = "تكييف وتبريد" },
                new Category { Id = 4, Name = "نقاشة ودهانات" },
                new Category { Id = 5, Name = "نجارة وصيانة أثاث" }
            );
        }

        // جداول الحسابات والتحقق الافتراضية
        public DbSet<EmailVerification> EmailVerifications { get; set; }
        public DbSet<PasswordResetOtp> PasswordResetOtps { get; set; }

        // جداول نظام الطلبات وعروض الأسعار (شغلك)
        public DbSet<JobRequest> JobRequests { get; set; }
        public DbSet<JobBid> JobBids { get; set; }
        public DbSet<Category> Categories { get; set; }

        // جداول نظام البروفايل والأوسطى (شغل صاحبك بعد التعديل)
        public DbSet<Craftsman> Craftsmen { get; set; }
    }
<<<<<<< HEAD
=======
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // تعريف مفتاح مركب للجدول الوسيط
        modelBuilder.Entity<CraftsmanProfession>()
            .HasKey(cp => new { cp.CraftsmanId, cp.ProfessionId });
    }

    public DbSet<EmailVerification> EmailVerifications { get; set; }
    public DbSet<PasswordResetOtp> PasswordResetOtps { get; set; }
    public DbSet<Plan> Plans { get; set; }
    public DbSet<UserSubscription> UserSubscriptions { get; set; }
    public DbSet<Payment> Payments { get; set; }   
    public DbSet<Craftsman> Craftsmen { get; set; }
    public DbSet<ApplicationUser> Applicationusers { get; set; }
    public DbSet <Category> Categories { get; set; }
    public DbSet<CraftsmanProfession> Craftsmanprofessions { get; set; }
    public DbSet<Profession> Professions { get; set; }
>>>>>>> 07f7e43df252c8299d81d9f3b49bff1a2b706906
}