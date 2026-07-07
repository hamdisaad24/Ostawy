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

        // 4. API سريع بالـ AJAX عشان يجيب العروض الجديدة أول بأول بدون ريفريش للصفحة
        [HttpGet]
        public async Task<IActionResult> GetBidsForRequest(int requestId)
        {
            var bids = await _db.JobBids
                .Where(b => b.JobRequestId == requestId && b.Status == "Pending")
                .Select(b => new {
                    b.Id,
                    b.OfferPrice,
                    b.Note,
                    CreatedAt = b.CreatedAt.ToString("yyyy-MM-dd hh:mm tt")
                }).ToListAsync();

            return Json(bids);
        }
    }
}