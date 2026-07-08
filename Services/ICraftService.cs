using System.Collections.Generic;
using System.Threading.Tasks;
using Ostawy.ViewModels;

namespace Ostawy.Services
{
    public interface ICraftService
    {
        Task<bool> CreateCraftAsync(CraftSubmissionViewModel model);
        Task<List<CraftIndexViewModel>> GetAllCraftsAsync();
    }
}
