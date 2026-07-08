using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Ostawy.Models;
using System;

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

            // 🛠️ السطر السحري: بنقول للـ EF لو لقيت جدول متكريت قبل كده، تجاهل الإيرور ده وكمل فرش باقي الجداول
            modelBuilder.Entity<Category>().ToTable("Categories", t => t.ExcludeFromMigrations());
        }

        // 1. جداول الحسابات والتحقق الافتراضية
        public DbSet<EmailVerification> EmailVerifications { get; set; }
        public DbSet<PasswordResetOtp> PasswordResetOtps { get; set; }

        // 2. جداول نظام الطلبات وعروض الأسعار (شغلك الموحد)
        public DbSet<JobRequest> JobRequests { get; set; }
        public DbSet<JobBid> JobBids { get; set; }

        // 3. جداول نظام البروفايل والأوسطى
        public DbSet<Craftsman> Craftsmen { get; set; }

        // 4. جداول نظام الخطط والاشتراكات والدفع الإلكتروني الجديدة
        public DbSet<Plan> Plans { get; set; }
        public DbSet<UserSubscription> UserSubscriptions { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Profession> Professions { get; set; }

        public DbSet<Category> Categories { get; set; }

        // Craft feature entities
        public DbSet<Craft> Crafts { get; set; }
        public DbSet<CraftImage> CraftImages { get; set; }

    }
}