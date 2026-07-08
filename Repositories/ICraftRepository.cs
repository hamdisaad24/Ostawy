using System.Threading.Tasks;
using Ostawy.Models;

namespace Ostawy.Repositories
{
    public interface ICraftRepository
    {
        Task AddCraftAsync(Craft craft);
    }
}
