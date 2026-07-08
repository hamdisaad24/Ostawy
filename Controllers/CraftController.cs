using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Ostawy.Services;
using Ostawy.ViewModels;

namespace Ostawy.Controllers
{
    public class CraftController : Controller
    {
        private readonly ICraftService _service;

        public CraftController(ICraftService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CraftSubmissionViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CraftSubmissionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _service.CreateCraftAsync(model);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "An error occurred while creating the craft.");
                return View(model);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
