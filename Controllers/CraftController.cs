using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Ostawy.Services;
using Ostawy.ViewModels;

namespace Ostawy.Controllers
{
    public class CraftController : Controller
    {
        private readonly ICraftService _service;
        private readonly ILogger<CraftController> _logger;

        public CraftController(ICraftService service, ILogger<CraftController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var items = await _service.GetAllCraftsAsync();
            return View(items);
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

            try
            {
                var ok = await _service.CreateCraftAsync(model);
                if (!ok)
                {
                    ModelState.AddModelError(string.Empty, "Could not save craft. Check inputs and try again.");
                    return View(model);
                }

                return RedirectToAction(nameof(Index));
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error creating craft");
                ModelState.AddModelError(string.Empty, "An unexpected error occurred while creating the craft.");
                return View(model);
            }
        }
    }
}
