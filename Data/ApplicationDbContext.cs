using Microsoft.EntityFrameworkCore;
using Ostawy.Models;

namespace Ostawy.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // السطر ده هو اللي هيخلي المشروع يشوف جدول الـ Categories
        public DbSet<Category> Categories { get; set; }

        public DbSet<User> Users { get; set; }
    }
}