using Microsoft.AspNetCore.Mvc;
using Ostawy.Services;
using Ostawy.ViewModels;

namespace Ostawy.Controllers
{
    public class ProfessionController : Controller
    {
        private readonly ProfessionService _professionService;

        public ProfessionController(ProfessionService professionService)
        {
            _professionService = professionService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var professions = await _professionService.GetAllAsync();
            return View(professions);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProfessionViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _professionService.CreateAsync(model);

            TempData["Success"] = "تم إضافة الحرفة بنجاح.";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var profession = await _professionService.GetByIdAsync(id);

            if (profession is null)
                return NotFound();

            return View(profession);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProfessionViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _professionService.UpdateAsync(model);

            TempData["Success"] = "تم تعديل الحرفة بنجاح.";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var profession = await _professionService.GetByIdAsync(id);

            if (profession is null)
                return NotFound();

            return View(profession);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _professionService.DeleteAsync(id);

            TempData["Success"] = "تم حذف الحرفة بنجاح.";

            return RedirectToAction(nameof(Index));
        }
    }
}
