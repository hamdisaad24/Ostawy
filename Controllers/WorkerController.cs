using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ostawy.Data;
using Ostawy.Models;
using System.Linq;
using System.Security.Claims;

namespace Ostawy.Controllers
{
    public class WorkerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WorkerController(ApplicationDbContext context)
        {
            _context = context;
        }


        //public IActionResult Dashboard()
        //{
        //    // 1. جلب الـ ID الحقيقي للفني اللي عامل تسجيل دخول حالياً
        //    var workerIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    if (string.IsNullOrEmpty(workerIdStr))
        //    {
        //        return RedirectToAction("Login", "Account"); // لو مش عامل دخول يرجعه لصفحة الـ Login
        //    }

        [HttpGet]
        public IActionResult CompleteProfile()
        {
            var workerIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(workerIdStr))
                return RedirectToAction("Login", "Account");

            int workerId = int.Parse(workerIdStr);
            var worker = _context.Users.Find(workerId);
            if (worker == null) return NotFound();

            return View(worker);
        }

        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> CompleteProfile(User model)
        // {
        //     var workerIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //     if (string.IsNullOrEmpty(workerIdStr))
        //         return RedirectToAction("Login", "Account");

        //     int workerId = int.Parse(workerIdStr);
        //     var worker = await _context.Users.FindAsync(workerId);
        //     if (worker == null) return NotFound();

        //     worker.Phone = model.Phone;
        //     worker.Address = model.Address;
        //     worker.Specialty = model.Specialty;
        //     worker.Price = model.Price;
        //     worker.Bio = model.Bio;
        //     worker.Lat = model.Lat;
        //     worker.Lng = model.Lng;

        //     await _context.SaveChangesAsync();

        //     return RedirectToAction("Dashboard");
        // }

        // public IActionResult Dashboard()
        // {
        //     // 1. جلب الـ ID الحقيقي للفني اللي عامل تسجيل دخول حالياً
        //     var workerIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //     if (string.IsNullOrEmpty(workerIdStr))
        //     {
        //         return RedirectToAction("Login", "Account"); // لو مش عامل دخول يرجعه لصفحة الـ Login
        //     }


        //    int workerId = int.Parse(workerIdStr);
        //    var worker = _context.Users.Find(workerId);

        //    if (worker == null) return NotFound();

        //    // 2. فحص هل كمل بياناته الشخصية؟
        //    bool isProfileIncomplete = string.IsNullOrEmpty(worker.Phone) ||
        //                               string.IsNullOrEmpty(worker.Specialty) ||
        //                               worker.Price == 0 ||
        //                               worker.Lat == null;

        //    // 3. إرسال البيانات الحقيقية للـ View عبر الـ ViewBag
        //    ViewBag.IsProfileIncomplete = isProfileIncomplete;
        //    ViewBag.WorkerName = worker.FullName;
        //    ViewBag.WorkerRating = worker.Rating; // التقييم الحقيقي من الجدول
        //    ViewBag.WorkerReviews = worker.ReviewsCount; // عدد التقييمات الحقيقي
        //    ViewBag.IsAvailable = worker.IsAvailable; // حالة التوصيل الحقيقية (True/False)

        //    // 4. جلب العدادات الحقيقية من جدول الـ Bids بناءً على الـ ID بتاعه
        //    ViewBag.ActiveBidsCount = _context.JobBids.Count(b => b.WorkerId == workerId && b.Status == "Pending");
        //    ViewBag.AcceptedJobsCount = _context.JobBids.Count(b => b.WorkerId == workerId && b.Status == "Accepted");

        //    return View(worker); // بعتنا كائن الـ worker نفسه كـ Model للصفحة
        //}

        //// أكشن تغيير حالة التوصيل (متصل / غير متصل) لايف بالـ Ajax
        //[HttpPost]
        //public IActionResult ToggleAvailability(bool available)
        //{
        //    var workerIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    if (string.IsNullOrEmpty(workerIdStr)) return Json(new { success = false });

        //    int workerId = int.Parse(workerIdStr);
        //    var worker = _context.Users.Find(workerId);

        //    if (worker != null)
        //    {
        //        worker.IsAvailable = available;
        //        _context.SaveChanges();
        //        return Json(new { success = true, status = available });
        //    }
        //    return Json(new { success = false });
        //}
    }
}