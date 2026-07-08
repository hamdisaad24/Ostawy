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

        // 2. استقبال بيانات الطلب ونشره (تم تعديله لمنع فشل الـ ModelState)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRequest(JobRequest model)
        {
            var clientId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(clientId)) return RedirectToAction("Login", "Account");

            // إسناد البيانات الأساسية للطلب خلف الكواليس
            model.ClientId = clientId;
            model.Status = "Open";
            model.CreatedAt = DateTime.Now;

            // 🛠️ الحركة السحرية: إزالة الحقول التي تسبب فشل التحقق التلقائي في الـ .NET 6/7/8
            ModelState.Remove("ClientId");
            ModelState.Remove("Category");

            if (ModelState.IsValid)
            {
                _db.JobRequests.Add(model);
                await _db.SaveChangesAsync();

                // التحويل المباشر لصفحة لوحة التحكم لمتابعة العروض لايف
                return RedirectToAction(nameof(RequestDashboard), new { requestId = model.Id });
            }

            // لو في أي مشكلة تانية حقيقية، بنعيد تحميل الأقسام عشان الفورم متضربش
            ViewBag.Categories = await _db.Categories.ToListAsync();
            return View(model);
        }

        // 3. لوحة تحكم الطلب (نصف لطلب العميل والنصف الثاني لعروض الصنايعية)
        public async Task<IActionResult> RequestDashboard(int requestId)
        {
            var request = await _db.JobRequests
                .Include(r => r.Category)
                .Include(r => r.JobBids)
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (request == null) return NotFound();

            return View(request);
        }

        // 4. API سريع بالـ AJAX عشان يجيب العروض الجديدة أول بأول مع إظهار اسم الصنايعي
        [HttpGet]
        public async Task<IActionResult> GetBidsForRequest(int requestId)
        {
            var bids = await _db.JobBids
                .Where(b => b.JobRequestId == requestId && b.Status == "Pending")
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            // حركة سحرية: بنجيب الاسماء من جدول الـ Users مباشرة عشان صاحبك مش عامل Navigation Property
            var result = bids.Select(b => new {
                id = b.Id,
                offerPrice = b.OfferPrice,
                note = b.Note,
                // بنروح ندور في جدول الـ Users على الاسم باستخدام الـ ArtisanId
                artisanName = _db.Users.FirstOrDefault(u => u.Id.ToString() == b.ArtisanId)?.FullName ?? "أسطى محترف",
                createdAt = b.CreatedAt.ToString("yyyy-MM-dd hh:mm tt")
            }).ToList();

            return Json(result);
        }

        // 5. 🚀 الـ API الجديد المسؤول عن قبول العرض لايف بالـ AJAX
        [HttpPost]
        public async Task<IActionResult> ApiAcceptBid(int bidId)
        {
            var bid = await _db.JobBids.Include(b => b.JobRequest).FirstOrDefaultAsync(b => b.Id == bidId);
            if (bid == null) return Json(new { success = false, message = "العرض غير موجود" });

            bid.Status = "Accepted";
            bid.JobRequest.Status = "Accepted"; // قفلنا الطلب وحولناه لمقبول

            // رفض باقي العروض التانية أوتوماتيك عشان الشغلانة اتقفلت خلاص
            var otherBids = await _db.JobBids.Where(b => b.JobRequestId == bid.JobRequestId && b.Id != bidId).ToListAsync();
            foreach (var other in otherBids)
            {
                other.Status = "Rejected";
            }

            await _db.SaveChangesAsync();
            return Json(new { success = true, message = "تم قبول العرض بنجاح! جاري إرسال بيانات التواصل للصنايعي. 🎉" });
        }

        // 6. 🚀 الـ API الجديد المسؤول عن رفض العرض لايف بالـ AJAX
        [HttpPost]
        public async Task<IActionResult> ApiRejectBid(int bidId)
        {
            var bid = await _db.JobBids.FindAsync(bidId);
            if (bid == null) return Json(new { success = false, message = "العرض غير موجود" });

            bid.Status = "Rejected"; // حولناه لمرفوض
            await _db.SaveChangesAsync();

            return Json(new { success = true, message = "تم رفض العرض بنجاح." });
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
        [Authorize(Roles = "Client")] // 🔐 دي اللي بتفرّق! بتسمح للعميل وتمنع الصنايعي تماماً
        [HttpGet]
        public async Task<IActionResult> MyRequests()
        {
            // بنجيب الـ ID بتاع العميل اللي مسجل دخول حالياً
            var clientId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(clientId)) return RedirectToAction("Login", "Account");

            // بنروح للداتا بيز نجيب الطلبات اللي العميل ده عملها هو بس
            var myRequests = await _db.JobRequests
                .Include(r => r.Category) // عشان نسحب اسم الحرفة (سباكة، كهرباء...)
                .Where(r => r.ClientId == clientId)
                .OrderByDescending(r => r.CreatedAt) // الأحدث يظهر فوق
                .ToListAsync();

            return View(myRequests);
        }
    }
}