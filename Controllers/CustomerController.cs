using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ostawy.Data;
using Ostawy.Models;

namespace Ostawy.Controllers
{
    [Authorize]
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CustomerController(ApplicationDbContext db)
        {
            _db = db;
        }

        // 1. عرض صفحة طلب صنايعي
        public async Task<IActionResult> CreateRequest()
        {
            ViewBag.Categories = await _db.Categories.ToListAsync();
            return View();
        }

        // 2. استقبال بيانات الطلب ونشره
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
            ModelState.Remove("Category");

            if (ModelState.IsValid)
            {
                _db.JobRequests.Add(model);
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(RequestDashboard), new { requestId = model.Id });
            }

            ViewBag.Categories = await _db.Categories.ToListAsync();
            return View(model);
        }

        // 3. لوحة تحكم الطلب
        public async Task<IActionResult> RequestDashboard(int requestId)
        {
            var request = await _db.JobRequests
                .Include(r => r.Category)
                .Include(r => r.JobBids)
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (request == null) return NotFound();

            return View(request);
        }

        // 4. API سريع بالـ AJAX يجلب العروض المفتوحة والمؤمنة ضد قفلات الـ LINQ
        [HttpGet]
        public async Task<IActionResult> GetBidsForRequest(int requestId)
        {
            var bids = await _db.JobBids
                .Where(b => b.JobRequestId == requestId && (b.Status == "Pending" || b.Status == "pending"))
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            var usersList = await _db.Users.ToListAsync();

            // سحب لستة الاشتراكات النشطة للـ PRO عشان نعلم عليهم
            var activeProUserIds = await _db.UserSubscriptions
                .Where(s => s.IsActive && s.Plan!.Name == "Pro")
                .Select(s => s.UserId.ToString())
                .ToListAsync();

            var result = bids.Select(b => new {
                id = b.Id,
                offerPrice = b.OfferPrice,
                note = b.Note,
                artisanName = usersList.FirstOrDefault(u => u.Id.ToString() == b.ArtisanId)?.FullName ?? "أسطى محترف",
                // 🚀 حركة سحرية: لو الـ ArtisanId موجود في لستة الـ PRO، نبعت علامة التوثيق
                isVerified = activeProUserIds.Contains(b.ArtisanId),
                createdAt = b.CreatedAt.ToString("yyyy-MM-dd hh:mm tt")
            }).ToList();

            return Json(result);
        }

        // 5. قبول العرض لايف بالـ AJAX ومتوافق مع كود الـ View بتاعك
        [HttpPost]
        public async Task<IActionResult> ApiAcceptBid(int bidId)
        {
            var bid = await _db.JobBids.Include(b => b.JobRequest).FirstOrDefaultAsync(b => b.Id == bidId);
            if (bid == null) return Json(new { success = false, message = "العرض غير موجود" });

            bid.Status = "Accepted";

            // عشان يطابق شروط الـ View التانية ويقفل الكارت، بنخلي حالة الشغلانة "Closed" أو "Accepted"
            if (bid.JobRequest != null)
            {
                bid.JobRequest.Status = "Closed";
            }

            // رفض باقي العروض التانية أوتوماتيك عشان الشغلانة اتقفلت خلاص
            var otherBids = await _db.JobBids.Where(b => b.JobRequestId == bid.JobRequestId && b.Id != bidId).ToListAsync();
            foreach (var other in otherBids)
            {
                other.Status = "Rejected";
            }

            await _db.SaveChangesAsync();
            return Json(new { success = true, message = "تم قبول العرض بنجاح! جاري إرسال بيانات التواصل للصنايعي. 🎉" });
        }

        // 6. رفض العرض لايف بالـ AJAX
        [HttpPost]
        public async Task<IActionResult> ApiRejectBid(int bidId)
        {
            var bid = await _db.JobBids.FindAsync(bidId);
            if (bid == null) return Json(new { success = false, message = "العرض غير موجود" });

            bid.Status = "Rejected";
            await _db.SaveChangesAsync();

            return Json(new { success = true, message = "تم رفض العرض بنجاح." });
        }

        // 🚨 8. الـ API الجديد المسؤول عن إلغاء الطلب بالكامل
        [HttpPost]
        public async Task<IActionResult> CancelRequest(int requestId)
        {
            var request = await _db.JobRequests.FindAsync(requestId);
            if (request == null) return Json(new { success = false, message = "الطلب غير موجود" });

            // تغيير حالة الطلب لـ ملغي
            request.Status = "Cancelled";

            // رفض جميع العروض المعلقة عليه تلقائياً
            var pendingBids = await _db.JobBids.Where(b => b.JobRequestId == requestId).ToListAsync();
            foreach (var bid in pendingBids)
            {
                bid.Status = "Rejected";
            }

            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }

        // 7. صفحة الإشعارات المفصولة ذكياً لكل مستخدم
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Notifications()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            if (User.IsInRole("Client"))
            {
                var clientNotifications = await _db.JobBids
                    .Include(b => b.JobRequest)
                    .ThenInclude(r => r.Category)
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
                    .ThenInclude(r => r.Category)
                    .Where(b => b.ArtisanId == userId && b.Status == "Accepted")
                    .OrderByDescending(b => b.CreatedAt)
                    .ToListAsync();

                ViewBag.UserRole = "CraftMan";
                return View(artisanNotifications);
            }

            return View(new List<JobBid>());
        }

        // 5. صفحة "طلباتي" الخاصة بالعميل فقط لمتابعة عروضها
        [Authorize(Roles = "Client")]
        [HttpGet]
        public async Task<IActionResult> MyRequests()
        {
            var clientId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(clientId)) return RedirectToAction("Login", "Account");

            var myRequests = await _db.JobRequests
                .Include(r => r.Category)
                .Where(r => r.ClientId == clientId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return View(myRequests);
        }
    }
}