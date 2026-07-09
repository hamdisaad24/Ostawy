using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ostawy.Data;
using Ostawy.Models;

namespace Ostawy.Controllers;

[Authorize]
public class CustomerController : Controller
{
    private readonly ApplicationDbContext _db;

    public CustomerController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public IActionResult CreateRequest()
    {
        ViewBag.Categories = _db.Professions.Select(p => new { Id = p.Id, Name = p.Name }).ToList();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateRequest(JobRequest model)
    {
        var clientId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(clientId)) return RedirectToAction("Login", "Account");

        model.ClientId = clientId;
        model.Status = "Open";
        model.CreatedAt = DateTime.Now;

        ModelState.Remove("ClientId");

        if (ModelState.IsValid)
        {
            _db.JobRequests.Add(model);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(RequestDashboard), new { requestId = model.Id });
        }

        ViewBag.Categories = _db.Professions.Select(p => new { Id = p.Id, Name = p.Name }).ToList();
        return View(model);
    }

    public async Task<IActionResult> RequestDashboard(int requestId)
    {
        var request = await _db.JobRequests
            .Include(r => r.JobBids)
            .FirstOrDefaultAsync(r => r.Id == requestId);

        if (request == null) return NotFound();

        return View(request);
    }

    [HttpGet]
    public async Task<IActionResult> GetBidsForRequest(int requestId)
    {
        var bids = await _db.JobBids
            .Where(b => b.JobRequestId == requestId && (b.Status == "Pending" || b.Status == "pending"))
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();

        var usersList = await _db.Users.ToListAsync();

        var activeProUserIds = await _db.UserSubscriptions
            .Where(s => s.IsActive && s.Plan!.Name == "Pro")
            .Select(s => s.UserId.ToString())
            .ToListAsync();

        var result = bids.Select(b => new {
            id = b.Id,
            offerPrice = b.OfferPrice,
            note = b.Note,
            artisanName = usersList.FirstOrDefault(u => u.Id.ToString() == b.ArtisanId)?.FullName ?? "أسطى محترف",
            isVerified = activeProUserIds.Contains(b.ArtisanId),
            createdAt = b.CreatedAt.ToString("yyyy-MM-dd hh:mm tt")
        }).ToList();

        return Json(result);
    }

    [HttpPost]
    public async Task<IActionResult> ApiAcceptBid(int bidId)
    {
        var bid = await _db.JobBids.Include(b => b.JobRequest).FirstOrDefaultAsync(b => b.Id == bidId);
        if (bid == null) return Json(new { success = false, message = "العرض غير موجود" });

        bid.Status = "Accepted";

        if (bid.JobRequest != null)
        {
            bid.JobRequest.Status = "Closed";
        }

        var otherBids = await _db.JobBids.Where(b => b.JobRequestId == bid.JobRequestId && b.Id != bidId).ToListAsync();
        foreach (var other in otherBids)
        {
            other.Status = "Rejected";
        }

        await _db.SaveChangesAsync();
        return Json(new { success = true, message = "تم قبول العرض بنجاح!" });
    }

    [HttpPost]
    public async Task<IActionResult> ApiRejectBid(int bidId)
    {
        var bid = await _db.JobBids.FindAsync(bidId);
        if (bid == null) return Json(new { success = false, message = "العرض غير موجود" });

        bid.Status = "Rejected";
        await _db.SaveChangesAsync();

        return Json(new { success = true, message = "تم رفض العرض بنجاح." });
    }

    [HttpPost]
    public async Task<IActionResult> CancelRequest(int requestId)
    {
        var request = await _db.JobRequests.FindAsync(requestId);
        if (request == null) return Json(new { success = false, message = "الطلب غير موجود" });

        request.Status = "Cancelled";

        var pendingBids = await _db.JobBids.Where(b => b.JobRequestId == requestId).ToListAsync();
        foreach (var bid in pendingBids)
        {
            bid.Status = "Rejected";
        }

        await _db.SaveChangesAsync();
        return Json(new { success = true });
    }

    [HttpGet]
    public async Task<IActionResult> Notifications()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        if (User.IsInRole("Client"))
        {
            var clientNotifications = await _db.JobBids
                .Include(b => b.JobRequest)
                .Where(b => b.JobRequest.ClientId == userId && b.Status == "Pending")
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            ViewBag.UserRole = "Client";
            return View(clientNotifications);
        }

        if (User.IsInRole("CraftMan"))
        {
            var artisanNotifications = await _db.JobBids
                .Include(b => b.JobRequest)
                .Where(b => b.ArtisanId == userId && b.Status == "Accepted")
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            ViewBag.UserRole = "CraftMan";
            return View(artisanNotifications);
        }

        return View(new List<JobBid>());
    }

    [Authorize(Roles = "Client")]
    [HttpGet]
    public async Task<IActionResult> MyRequests()
    {
        var clientId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(clientId)) return RedirectToAction("Login", "Account");

        var myRequests = await _db.JobRequests
            .Where(r => r.ClientId == clientId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return View(myRequests);
    }
}
