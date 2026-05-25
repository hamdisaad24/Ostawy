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
    }
}