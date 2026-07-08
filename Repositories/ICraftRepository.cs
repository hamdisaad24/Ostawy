using System.Collections.Generic;
using System.Threading.Tasks;
using Ostawy.Models;

namespace Ostawy.Repositories
{
    public interface ICraftRepository
    {
        Task AddAsync(Craft craft);
        Task<List<Craft>> GetAllAsync();
    }
}
