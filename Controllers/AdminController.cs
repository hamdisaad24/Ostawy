using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ostawy.Data;

namespace Ostawy.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Requests()
    {
        var requests = await _context.JobRequests
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
        return View(requests);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var request = await _context.JobRequests.FindAsync(id);
        if (request is null)
            return NotFound();
        return View(request);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var request = await _context.JobRequests.FindAsync(id);
        if (request is null)
            return NotFound();

        _context.JobRequests.Remove(request);
        await _context.SaveChangesAsync();

        TempData["Success"] = "تم حذف الطلب بنجاح.";
        return RedirectToAction(nameof(Requests));
    }
}
