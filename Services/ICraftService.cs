using System.Threading.Tasks;
using Ostawy.ViewModels;

namespace Ostawy.Services
{
    public interface ICraftService
    {
        /// <summary>
        /// Creates a craft entry including saving files to disk and persisting DB records.
        /// Returns tuple (Success, ErrorMessage)
        /// </summary>
        Task<(bool Success, string ErrorMessage)> CreateCraftAsync(CraftSubmissionViewModel model);
    }
}
