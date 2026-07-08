using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ostawy.Data;
using Ostawy.Models;

namespace Ostawy.Repositories
{
    public class CraftRepository : ICraftRepository
    {
        private readonly ApplicationDbContext _db;

        public CraftRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Craft craft)
        {
            _db.Crafts.Add(craft);
            await _db.SaveChangesAsync();
        }

        public async Task<List<Craft>> GetAllAsync()
        {
            return await _db.Crafts
                .Include(c => c.GalleryImages)
                .ToListAsync();
        }
    }
}
