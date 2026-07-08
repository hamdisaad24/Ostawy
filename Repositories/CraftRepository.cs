using System.Threading.Tasks;
using Ostawy.Models;
using Ostawy.Data;

namespace Ostawy.Repositories
{
    public class CraftRepository : ICraftRepository
    {
        private readonly ApplicationDbContext _db;

        public CraftRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task AddCraftAsync(Craft craft)
        {
            _db.Crafts.Add(craft);
            await _db.SaveChangesAsync();
        }
    }
}
