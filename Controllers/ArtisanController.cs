using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ostawy.Data;
using Ostawy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Ostawy.Controllers
{
    [Authorize] // تأمين الصفحة
    public class ArtisanController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> userManager;

        public ArtisanController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            this.userManager = userManager;
        }

        public async Task<IActionResult> AvailableJobs()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString());
            if (userId == Guid.Empty) return RedirectToAction("Login", "Account");

            var jobs = await _db.JobRequests
                .OrderByDescending(j => j.CreatedAt)
                .ToListAsync();

            var submittedJobIds = await _db.JobBids
                .Where(b => b.UserId == userId)
                .Select(b => b.JobRequestId)
                .ToListAsync();

            ViewBag.SubmittedJobIds = submittedJobIds;
            var user = await userManager.FindByIdAsync(userId.ToString());
            ViewBag.PhoneNumber = user!.PhoneNumber;

            return View(jobs);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitBid(Guid JobRequestId, decimal OfferPrice, string Note)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString());
            if (userId == Guid.Empty) return RedirectToAction("Login", "Account");

            var existingBid = await _db.JobBids
                .AnyAsync(b => b.JobRequestId == JobRequestId && b.UserId == userId);

            if (existingBid)
            {
                TempData["Error"] = "لقد قمت بتقديم عرض على هذا الطلب بالفعل!";
                return RedirectToAction(nameof(AvailableJobs));
            }

            bool isProUser = false;
            isProUser = await _db.UserSubscriptions
                .AnyAsync(s => s.UserId == userId && s.IsActive && s.Plan!.Name == "Pro");

            if (!isProUser)
            {
                var currentMonth = DateTime.Now.Month;
                var currentYear = DateTime.Now.Year;

                var bidsCountThisMonth = await _db.JobBids
                    .CountAsync(b => b.UserId == userId &&
                                     b.CreatedAt.Month == currentMonth &&
                                     b.CreatedAt.Year == currentYear);

                if (bidsCountThisMonth >= 5)
                {
                    TempData["Error"] = "🚨 لقد استهلكت حدك المجاني (5 عروض شهرياً)! يرجى الترقية إلى الباقة المتميزة بـ 150 ج.م لإرسال عروض غير محدودة وحصولك على شارة التوثيق. 🚀";
                    return RedirectToAction(nameof(AvailableJobs));
                }
            }

            var bid = new JobBid
            {
                JobRequestId = JobRequestId,
                UserId = userId,
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

        [HttpGet]
        public async Task<IActionResult> Notifications()
        {
            var artisanId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(artisanId)) return RedirectToAction("Login", "Account");

            var realNotifications = new List<dynamic>();

            try
            {
                var allBids = await _db.JobBids
                    .Include(b => b.JobRequest)
                    .OrderByDescending(b => b.CreatedAt)
                    .ToListAsync();

                var myBids = allBids
                    .Where(b => b.UserId == Guid.Parse(artisanId))
                    .ToList();

                foreach (var bid in myBids)
                {
                    if (bid.Status == "Accepted")
                    {
                        realNotifications.Add(new
                        {
                            Title = "🎉 مبروك! تم قبول عرضك",
                            Message = $"وافق العميل على عرض السعر الخاص بك ({bid.OfferPrice} ج.م) لطلب '{bid.JobRequest?.Description}'. تواصل معه الآن!",
                            CreatedAt = bid.CreatedAt
                        });
                    }
                    else if (bid.Status == "Rejected")
                    {
                        realNotifications.Add(new
                        {
                            Title = "⚠️ تم رفض العرض",
                            Message = $"عذراً، رفض العميل عرضك المقدم لطلب '{bid.JobRequest?.Description}'. خيرها في غيرها!",
                            CreatedAt = bid.CreatedAt
                        });
                    }
                }
            }
            catch { }

            if (Guid.TryParse(artisanId, out Guid artisanGuid))
            {
                try
                {
                    var artisan = await _db.Craftsmen
                        .FirstOrDefaultAsync(c => c.UserId == artisanGuid);

                    if (artisan != null)
                    {
                        var matchedJobs = await _db.JobRequests
                            .Where(j => j.Status == "Open")
                            .OrderByDescending(j => j.CreatedAt)
                            .Take(5)
                            .ToListAsync();

                        foreach (var job in matchedJobs)
                        {
                            realNotifications.Add(new
                            {
                                Title = "🎯 شغلانة جديدة متوفرة!",
                                Message = $"طلب جديد متاح الآن: '{job.Description}' بميزانية مقترحة {job.EstimatedPrice} ج.م. قدّم عرضك بسرعة!",
                                CreatedAt = job.CreatedAt
                            });
                        }
                    }
                }
                catch { }
            }

            var sortedNotifications = realNotifications.OrderByDescending(n => n.CreatedAt).ToList();

            return View(sortedNotifications);
        }

        [HttpGet]
        public IActionResult MyWorks()
        {
            var artisanId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(artisanId)) return RedirectToAction("Login", "Account");

            var mockWorks = new List<dynamic>
            {
                 new {
                     Title = "تأسيس شبكة سباكة متكاملة للشقة",
                     Description = "تم تأسيس مواسير التغذية والصرف بالكامل باستخدام خامات عالية الجودة مع عمل اختبار كبس للمواسير وضمان لمدة 5 سنوات.",
                     ClientName = "أحمد رأفت",
                     Rating = 5,
                     ImageName = "plumbing_work.jpg",
                     Date = "2026-05-12"
                 },
                 new {
                     Title = "تركيب وتجهيز غرف حمامات حديثة",
                     Description = "تركيب حوض، وقاعدة معلقة، وخلاطات دفن ذكية مع عزل الأرضيات تماماً ضد تسريب المياه.",
                     ClientName = "مهندس محمد صلاح",
                     Rating = 4,
                     ImageName = "bathroom_work.jpg",
                     Date = "2026-06-01"
                 },
                 new {
                     Title = "إصلاح شبكة صرف خارجي لعمارة",
                     Description = "تغيير خطوط الصرف الرئيسية التالفة وعمل غرف تفتيش جديدة بالكامل لحل مشكلة الانسداد المزمن.",
                     ClientName = "الحاج مصطفى عابدين",
                     Rating = 5,
                     ImageName = "drainage_work.jpg",
                     Date = "2026-06-25"
                 }
            };

            return View(mockWorks);
        }
    }
}