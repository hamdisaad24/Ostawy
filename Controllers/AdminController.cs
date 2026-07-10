using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ostawy.Data;
using Ostawy.Models;
using Ostawy.ViewModels;

namespace Ostawy.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Requests()
    {
        var requests = await _context.JobRequests
            .Include(r => r.User)
            .Include(r => r.Profession)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return View(requests);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(Guid id)
    {
        var request = await _context.JobRequests
            .Include(r => r.User)
            .Include(r => r.Profession)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (request == null)
            return NotFound();

        return View(request);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var request = await _context.JobRequests.FindAsync(id);

        if (request == null)
            return NotFound();

        _context.JobRequests.Remove(request);

        await _context.SaveChangesAsync();

        TempData["Success"] = "تم حذف الطلب بنجاح.";

        return RedirectToAction(nameof(Requests));
    }
    [HttpGet]
    public async Task<IActionResult> Clients()
    {
        var users = await _userManager.Users.ToListAsync();

        var clients = new List<ApplicationUser>();

        foreach (var user in users)
        {
            if (await _userManager.IsInRoleAsync(user, "Client"))
            {
                clients.Add(user);
            }
        }

        return View(clients);
    }



    // =========================
    // Artisans
    // =========================

    [HttpGet]
    public async Task<IActionResult> Artisans()
    {
        var users = await _userManager.Users.ToListAsync();

        var artisans = new List<ApplicationUser>();

        foreach (var user in users)
        {
            if (await _userManager.IsInRoleAsync(user, "CraftMan"))
            {
                artisans.Add(user);
            }
        }

        return View(artisans);
    }



    // =========================
    // User Details
    // =========================

    [HttpGet]
    public async Task<IActionResult> UserDetails(Guid id)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == id);

        if (user == null)
            return NotFound();


        var requests = await _context.JobRequests
            .Where(x => x.UserId == id)
            .Include(x => x.Profession)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();


        var bids = await _context.JobBids
            .Where(x => x.UserId == id)
            .Include(x => x.JobRequest)
            .ThenInclude(x => x!.Profession)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();


        var isPro = await _context.UserSubscriptions
            .AnyAsync(x =>
                x.UserId == id &&
                x.IsActive &&
                x.Plan!.Name == "PRO المتميزة");


        var model = new UserDetailsViewModel
        {
            User = user,

            RequestsCount = requests.Count,

            BidsCount = bids.Count,

            AcceptedBidsCount = bids.Count(x => x.Status == "Accepted"),

            IsPro = isPro,

            Requests = requests,

            Bids = bids
        };


        return View(model);
    }



    // =========================
    // Delete User
    // =========================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUser(Guid id)
    {

        var user = await _userManager.FindByIdAsync(id.ToString());

        if (user == null)
            return NotFound();


        var result = await _userManager.DeleteAsync(user);


        if (result.Succeeded)
        {
            TempData["Success"] = "تم حذف المستخدم بنجاح";
        }


        return RedirectToAction(nameof(Clients));
    }

}
