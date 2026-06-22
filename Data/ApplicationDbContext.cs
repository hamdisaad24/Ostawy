using Microsoft.EntityFrameworkCore;
using Ostawy.Models;

namespace Ostawy.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // السطر ده معناه: اعملي جدول اسمه Users بناءً على كلاس User
        public DbSet<User> Users { get; set; }

        // دالة الـ OnModelCreating دي بنستخدمها عشان نملى الداتا بيز ببيانات أولية (Seed Data)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, FullName = "أحمد محمد", Email = "ahmed@ostawy.com", PasswordHash = "123456", Role = "worker", Specialty = "سباكة", Category = "plumbing", Lat = 30.0444, Lng = 31.2357, IsAvailable = true, Price = 80, Rating = 4.9, ReviewsCount = 128 },
                new User { Id = 2, FullName = "محمود خالد", Email = "mahmoud@ostawy.com", PasswordHash = "123456", Role = "worker", Specialty = "كهرباء", Category = "electric", Lat = 30.0600, Lng = 31.2600, IsAvailable = true, Price = 100, Rating = 4.8, ReviewsCount = 97 },
                new User { Id = 3, FullName = "سامر علي", Email = "samer@ostawy.com", PasswordHash = "123456", Role = "worker", Specialty = "تكييف وتبريد", Category = "ac", Lat = 30.0350, Lng = 31.2200, IsAvailable = true, Price = 150, Rating = 4.7, ReviewsCount = 64 },
                new User { Id = 4, FullName = "يوسف حسن", Email = "youssef_h@ostawy.com", PasswordHash = "123456", Role = "worker", Specialty = "دهانات", Category = "paint", Lat = 30.0700, Lng = 31.2800, IsAvailable = true, Price = 70, Rating = 4.9, ReviewsCount = 211 }
            );
        }
        public DbSet<Order> Orders { get; set; }
        public DbSet<JobRequest> JobRequests { get; set; }
        public DbSet<JobBid> JobBids { get; set; }
    }
}