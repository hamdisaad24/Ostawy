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
<<<<<<< HEAD
        public DbSet<CraftManProfessionImage> CraftManProfessionImages { get; set; }
=======
        public DbSet<JobRequest> JobRequests { get; set; }
        public DbSet<JobBid> JobBids { get; set; }
>>>>>>> e6e5dfec399d24b6061d61e5d39dd64881de0979
    }
}