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

        // السطر ده معناه: اعملي جدول اسمه Users بناءً على كلاس User
        public DbSet<User> Users { get; set; }
    }
    public DbSet<EmailVerification> EmailVerifications { get; set; }
    public DbSet<PasswordResetOtp> PasswordResetOtps { get; set; }

}