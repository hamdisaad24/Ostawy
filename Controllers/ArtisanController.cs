using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ostawy.Data;
using Ostawy.Models;
using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Ostawy.Controllers
{
    [Authorize] // تأمين الصفحة
    public class ArtisanController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ArtisanController(ApplicationDbContext db)
        {
            _db = db;
        }

        // 1. عرض الطلبات المتاحة للصنايعية
        public async Task<IActionResult> AvailableJobs()
        {
            // جلب كل الطلبات المفتوحة حالياً عشان الصنايعي يتصفحها
            var jobs = await _db.JobRequests
                .Include(j => j.Category)
                .Where(j => j.Status == "Open")
                .OrderByDescending(j => j.CreatedAt)
                .ToListAsync();

            return View(jobs);
        }

        // 2. استقبال عرض السعر (Bid) من الصنايعي
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitBid(int JobRequestId, decimal OfferPrice, string Note)
        {
            var artisanId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(artisanId)) return RedirectToAction("Login", "Account");

            // التأكد إن الصنايعي مقدمش عرض قبل كده على نفس الشغلانة منعاً للتكرار
            var existingBid = await _db.JobBids
                .AnyAsync(b => b.JobRequestId == JobRequestId && b.ArtisanId == artisanId);

            if (existingBid)
            {
                TempData["Error"] = "لقد قمت بتقديم عرض على هذا الطلب بالفعل!";
                return RedirectToAction(nameof(AvailableJobs));
            }

            // إنشاء العرض الجديد
            var bid = new JobBid
            { 
                JobRequestId = JobRequestId,
                ArtisanId = artisanId,
                OfferPrice = OfferPrice,
                Note = Note,
                Status = "Pending",
                CreatedAt = DateTime.Now
            };

        _db.JobBids.Add(bid);
            await _db.SaveChangesAsync();

        TempData["Success"] = "تم إرسال عرضك بنجاح إلى العميل! تابع الصفحة بانتظار قبوُله.";
            return RedirectToAction(nameof(AvailableJobs));
    }
}
}