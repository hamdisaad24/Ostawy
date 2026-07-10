using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        ViewBag.Professions = new SelectList(
            _db.Professions,
            "Id",
            "Name");

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateRequest(JobRequest model)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return RedirectToAction("Login", "Account");

        if (!ModelState.IsValid)
        {
            ViewBag.Professions = new SelectList(
                _db.Professions,
                "Id",
                "Name");

            return View(model);
        }

        model.Id = Guid.NewGuid();
        model.UserId = Guid.Parse(userId);
        model.Status = "Open";
        model.CreatedAt = DateTime.UtcNow;

        _db.JobRequests.Add(model);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(RequestDashboard), new { requestId = model.Id });
    }

    [HttpGet]
    public async Task<IActionResult> RequestDashboard(Guid requestId)
    {
        var request = await _db.JobRequests
            .Include(r => r.User)
            .Include(r => r.Profession)
            .Include(r => r.JobBids)
            .FirstOrDefaultAsync(r => r.Id == requestId);

        if (request == null)
            return NotFound();

        ViewBag.AcceptedBid = request.JobBids?
            .FirstOrDefault(b => b.Status == "Accepted");

        return View(request);
    }

    [HttpGet]
    public async Task<IActionResult> GetBidsForRequest(Guid requestId)
    {
        var bids = await _db.JobBids
            .Where(b => b.JobRequestId == requestId &&
                        b.Status == "Pending")
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();

        var activeProUsers = await _db.UserSubscriptions
            .Include(x => x.Plan)
            .Where(x => x.IsActive && x.Plan!.Name == "Pro")
            .Select(x => x.UserId)
            .ToListAsync();

        var result = bids.Select(b => new
        {
            id = b.Id,
            offerPrice = b.OfferPrice,
            note = b.Note,

            artisanName = _db.Users
                .Where(u => u.Id == b.UserId)
                .Select(u => u.FullName)
                .FirstOrDefault() ?? "أسطى محترف",

            phoneNumber = _db.Users
                .Where(u => u.Id == b.UserId)
                .Select(u => u.PhoneNumber)
                .FirstOrDefault(),

            isVerified = activeProUsers.Contains(b.UserId),

            status = b.Status,

            createdAt = b.CreatedAt.ToString("yyyy-MM-dd hh:mm tt")
        });

        return Json(result);
    }

    [HttpPost]
    public async Task<IActionResult> ApiAcceptBid(Guid bidId)
    {
        var bid = await _db.JobBids
            .Include(b => b.JobRequest)
            .FirstOrDefaultAsync(b => b.Id == bidId);

        if (bid == null)
            return Json(new
            {
                success = false,
                message = "العرض غير موجود"
            });

        if (bid.JobRequest == null)
            return Json(new
            {
                success = false,
                message = "الطلب غير موجود"
            });

        if (bid.JobRequest.Status != "Open")
            return Json(new
            {
                success = false,
                message = "تم اختيار صنايعي بالفعل."
            });

        bid.Status = "Accepted";

        bid.JobRequest.Status = "Assigned";

        var otherBids = await _db.JobBids
            .Where(x => x.JobRequestId == bid.JobRequestId &&
                        x.Id != bid.Id)
            .ToListAsync();

        foreach (var other in otherBids)
        {
            other.Status = "Rejected";
        }

        await _db.SaveChangesAsync();

        return Json(new
        {
            success = true,
            message = "تم قبول العرض بنجاح."
        });
    }

    [HttpPost]
    public async Task<IActionResult> ApiRejectBid(Guid bidId)
    {
        var bid = await _db.JobBids.FindAsync(bidId);

        if (bid == null)
            return Json(new
            {
                success = false,
                message = "العرض غير موجود"
            });

        bid.Status = "Rejected";

        await _db.SaveChangesAsync();

        return Json(new
        {
            success = true,
            message = "تم رفض العرض."
        });
    }

    [HttpPost]
    public async Task<IActionResult> CompleteRequest(Guid requestId)
    {
        var request = await _db.JobRequests.FindAsync(requestId);

        if (request == null)
            return Json(new
            {
                success = false,
                message = "الطلب غير موجود"
            });

        request.Status = "Completed";

        await _db.SaveChangesAsync();

        return Json(new
        {
            success = true,
            message = "تم إنهاء الطلب بنجاح."
        });
    }

    [HttpPost]
    public async Task<IActionResult> CancelRequest(Guid requestId)
    {
        var request = await _db.JobRequests.FindAsync(requestId);

        if (request == null)
            return Json(new
            {
                success = false,
                message = "الطلب غير موجود"
            });

        request.Status = "Cancelled";

        var bids = await _db.JobBids
            .Where(b => b.JobRequestId == requestId)
            .ToListAsync();

        foreach (var bid in bids)
        {
            bid.Status = "Rejected";
        }

        await _db.SaveChangesAsync();

        return Json(new { success = true });
    }

    [HttpGet]
    public async Task<IActionResult> Notifications()
    {
        Guid userId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        if (User.IsInRole("Client"))
        {
            var notifications = await _db.JobBids
                .Include(b => b.JobRequest)
                .Where(b => b.JobRequest!.UserId == userId &&
                            b.Status == "Pending")
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            ViewBag.UserRole = "Client";

            return View(notifications);
        }

        if (User.IsInRole("CraftMan"))
        {
            var notifications = await _db.JobBids
                .Include(b => b.JobRequest)
                .Where(b => b.UserId == userId &&
                            b.Status == "Accepted")
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            ViewBag.UserRole = "CraftMan";

            return View(notifications);
        }

        return View(new List<JobBid>());
    }

    [Authorize(Roles = "Client")]
    [HttpGet]
    public async Task<IActionResult> MyRequests()
    {
        var userId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var requests = await _db.JobRequests
            .Include(r => r.Profession)
            .Include(r => r.JobBids)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return View(requests);
    }
}