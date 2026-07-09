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
        public DbSet<EmailVerification> EmailVerifications { get; set; }
        public DbSet<PasswordResetOtp> PasswordResetOtps { get; set; }
        public DbSet<Plan> Plans { get; set; }
        public DbSet<UserSubscription> UserSubscriptions { get; set; }
        public DbSet<Payment> Payments { get; set; }   
        public DbSet<Craftsman> Craftsmen { get; set; }
        public DbSet<Profession> Professions { get; set; }
        public DbSet<CraftManProfession> CraftManProfessions { get; set; }
        public DbSet<CraftManProfessionImage> CraftManProfessionImages { get; set; }
    }
}