using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ostawy.Services;
using Ostawy.ViewModels;

namespace Ostawy.Controllers;


[Authorize(Roles = "Admin")]
public class PlanController : Controller
{
    private readonly PlanService _planService;

    public PlanController(PlanService planService)
    {
        _planService = planService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var plans = await _planService.GetAllAsync();
        return View(plans);
    }


    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PlanViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        await _planService.CreateAsync(model);

        TempData["Success"] = "تم إضافة الباقة بنجاح.";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var plan = await _planService.GetByIdAsync(id);

        if (plan is null)
            return NotFound();

        return View(plan);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(PlanViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var updated = await _planService.UpdateAsync(model);

        if (!updated)
            return NotFound();

        TempData["Success"] = "تم تعديل الباقة بنجاح.";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(Guid id)
    {
        var plan = await _planService.GetByIdAsync(id);

        if (plan is null)
            return NotFound();

        return View(plan);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var deleted = await _planService.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        TempData["Success"] = "تم حذف الباقة بنجاح.";

        return RedirectToAction(nameof(Index));
    }
}