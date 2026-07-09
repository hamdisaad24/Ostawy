// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
// using Ostawy.Data;
// using Ostawy.Models;
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Security.Claims;
// using System.Threading.Tasks;

// namespace Ostawy.Controllers
// {
//     [Authorize] // تأمين الصفحة
//     public class ArtisanController : Controller
//     {
//         private readonly ApplicationDbContext _db;

//         public ArtisanController(ApplicationDbContext db)
//         {
//             _db = db;
//         }

//         // 1. عرض الطلبات المتاحة للصنايعية مع فحص الحالة
//         public async Task<IActionResult> AvailableJobs()
//         {
//             var artisanId = User.FindFirstValue(ClaimTypes.NameIdentifier);
//             if (string.IsNullOrEmpty(artisanId)) return RedirectToAction("Login", "Account");

//             // جلب كل الطلبات المتاحة (سواء كانت مفتوحة أو منتهية عشان تظهر الحالات كلها في المعرض)
//             var jobs = await _db.JobRequests
//                 .Include(j => j.Category)
//                 .OrderByDescending(j => j.CreatedAt)
//                 .ToListAsync();

//             // 🚀 سحب الـ IDs بتاعة الطلبات اللي الصنايعي ده قدم عليها عرض سعر قبل كده
//             var submittedJobIds = await _db.JobBids
//                 .Where(b => b.ArtisanId == artisanId)
//                 .Select(b => b.JobRequestId)
//                 .ToListAsync();

//             // تمرير اللستة للـ View عبر الـ ViewBag
//             ViewBag.SubmittedJobIds = submittedJobIds;

//             return View(jobs);
//         }

//         // 2. استقبال عرض السعر (Bid) من الصنايعي
//         [HttpPost]
//         [ValidateAntiForgeryToken]
//         public async Task<IActionResult> SubmitBid(int JobRequestId, decimal OfferPrice, string Note)
//         {
//             var artisanId = User.FindFirstValue(ClaimTypes.NameIdentifier);
//             if (string.IsNullOrEmpty(artisanId)) return RedirectToAction("Login", "Account");

//             // 1. التأكد إن الصنايعي مقدمش عرض قبل كده على نفس الشغلانة منعاً للتكرار
//             var existingBid = await _db.JobBids
//                 .AnyAsync(b => b.JobRequestId == JobRequestId && b.ArtisanId == artisanId);

//             if (existingBid)
//             {
//                 TempData["Error"] = "لقد قمت بتقديم عرض على هذا الطلب بالفعل!";
//                 return RedirectToAction(nameof(AvailableJobs));
//             }

//             // 🚀 2. لوجيك الباقات الذكي: تشييك هل المستخدم حساب PRO نشط؟
//             bool isProUser = false;
//             if (Guid.TryParse(artisanId, out Guid userId))
//             {
//                 isProUser = await _db.UserSubscriptions
//                     .AnyAsync(s => s.UserId == userId && s.IsActive && s.Plan!.Name == "Pro");
//             }

//             // 🛑 3. لو مش PRO، نعد عروضه الشغالة في الشهر الحالي
//             if (!isProUser)
//             {
//                 var currentMonth = DateTime.Now.Month;
//                 var currentYear = DateTime.Now.Year;

//                 // بنعد العروض اللي قدمها في الشهر الحالي
//                 var bidsCountThisMonth = await _db.JobBids
//                     .CountAsync(b => b.ArtisanId == artisanId &&
//                                      b.CreatedAt.Month == currentMonth &&
//                                      b.CreatedAt.Year == currentYear);

//                 if (bidsCountThisMonth >= 5)
//                 {
//                     TempData["Error"] = "🚨 لقد استهلكت حدك المجاني (5 عروض شهرياً)! يرجى الترقية إلى الباقة المتميزة بـ 150 ج.م لإرسال عروض غير محدودة وحصولك على شارة التوثيق. 🚀";
//                     return RedirectToAction(nameof(AvailableJobs));
//                 }
//             }

//             // 4. إنشاء العرض الجديد لو الشرط تمام
//             var bid = new JobBid
//             {
//                 JobRequestId = JobRequestId,
//                 ArtisanId = artisanId,
//                 OfferPrice = OfferPrice,
//                 Note = Note,
//                 Status = "Pending",
//                 CreatedAt = DateTime.Now
//             };

//             _db.JobBids.Add(bid);
//             await _db.SaveChangesAsync();

//             TempData["Success"] = "تم إرسال عرضك بنجاح إلى العميل! تابع الصفحة بانتظار قبوُله.";
//             return RedirectToAction(nameof(AvailableJobs));
//         }

//         [HttpGet]
//         public async Task<IActionResult> Notifications()
//         {
//             var artisanId = User.FindFirstValue(ClaimTypes.NameIdentifier);
//             if (string.IsNullOrEmpty(artisanId)) return RedirectToAction("Login", "Account");

//             // لستة ديناميكية هنجمع فيها الإشعارات في الـ Memory
//             var realNotifications = new List<dynamic>();

//             try
//             {
//                 // 1. جلب كل العروض المربوطة بـ الـ JobRequest في الميموري أولاً لمنع تعارض الـ SQL translation
//                 var allBids = await _db.JobBids
//                     .Include(b => b.JobRequest)
//                     .OrderByDescending(b => b.CreatedAt)
//                     .ToListAsync(); // 👈 سحب فوري للميموري بأمان

//                 // الفلترة جوه الميموري
//                 var myBids = allBids
//                     .Where(b => b.ArtisanId == artisanId)
//                     .ToList();

//                 foreach (var bid in myBids)
//                 {
//                     if (bid.Status == "Accepted")
//                     {
//                         realNotifications.Add(new
//                         {
//                             Title = "🎉 مبروك! تم قبول عرضك",
//                             Message = $"وافق العميل على عرض السعر الخاص بك ({bid.OfferPrice} ج.م) لطلب '{bid.JobRequest?.Description}'. تواصل معه الآن!",
//                             CreatedAt = bid.CreatedAt
//                         });
//                     }
//                     else if (bid.Status == "Rejected")
//                     {
//                         realNotifications.Add(new
//                         {
//                             Title = "⚠️ تم رفض العرض",
//                             Message = $"عذراً، رفض العميل عرضك المقدم لطلب '{bid.JobRequest?.Description}'. خيرها في غيرها!",
//                             CreatedAt = bid.CreatedAt
//                         });
//                     }
//                 }
//             }
//             catch { }

//             // 2. سحب الطلبات الجديدة المتوافقة مع تخصص الأوسطى بأمان تام
//             try
//             {
//                 var artisan = await _db.Craftsmen
//                     .FirstOrDefaultAsync();

//                 if (artisan != null)
//                 {
//                     int targetCategory = artisan.CategoryId;

//                     // بنجيب أحدث طلبات مفتوحة صريحة للميموري أولاً
//                     var allOpenJobs = await _db.JobRequests
//                         .Where(j => j.Status == "Open")
//                         .OrderByDescending(j => j.CreatedAt)
//                         .ToListAsync();

//                     // الفلترة في الميموري بعيداً عن تعقيدات الـ SQL
//                     var matchedJobs = allOpenJobs
//                         .Where(j => j.CategoryId == targetCategory)
//                         .Take(5)
//                         .ToList();

//                     foreach (var job in matchedJobs)
//                     {
//                         realNotifications.Add(new
//                         {
//                             Title = "🎯 شغلانة جديدة متوفرة في تخصصك!",
//                             Message = $"طلب جديد متاح الآن: '{job.Description}' بميزانية مقترحة {job.EstimatedPrice} ج.م. قدّم عرضك بسرعة!",
//                             CreatedAt = job.CreatedAt
//                         });
//                     }
//                 }
//             }
//             catch { }

//             // ترتيب الإشعارات كلها من الأحدث للأقدم
//             var sortedNotifications = realNotifications.OrderByDescending(n => n.CreatedAt).ToList();

//             return View(sortedNotifications);
//         }

//         // 4. صفحة أعمالي الخاصة بالصنايعي (معرض الأعمال السابقة)
//         [HttpGet]
//         public IActionResult MyWorks()
//         {
//             var artisanId = User.FindFirstValue(ClaimTypes.NameIdentifier);
//             if (string.IsNullOrEmpty(artisanId)) return RedirectToAction("Login", "Account");

//             // داتا ثابتة احترافية جداً لتمثيل معرض الأعمال لايف
//             var mockWorks = new List<dynamic>
//     {
//         new {
//             Title = "تأسيس شبكة سباكة متكاملة للشقة",
//             Description = "تم تأسيس مواسير التغذية والصرف بالكامل باستخدام خامات عالية الجودة مع عمل اختبار كبس للمواسير وضمان لمدة 5 سنوات.",
//             ClientName = "أحمد رأفت",
//             Rating = 5,
//             ImageName = "plumbing_work.jpg",
//             Date = "2026-05-12"
//         },
//         new {
//             Title = "تركيب وتجهيز غرف حمامات حديثة",
//             Description = "تركيب حوض، وقاعدة معلقة، وخلاطات دفن ذكية مع عزل الأرضيات تماماً ضد تسريب المياه.",
//             ClientName = "مهندس محمد صلاح",
//             Rating = 4,
//             ImageName = "bathroom_work.jpg",
//             Date = "2026-06-01"
//         },
//         new {
//             Title = "إصلاح شبكة صرف خارجي لعمارة",
//             Description = "تغيير خطوط الصرف الرئيسية التالفة وعمل غرف تفتيش جديدة بالكامل لحل مشكلة الانسداد المزمن.",
//             ClientName = "الحاج مصطفى عابدين",
//             Rating = 5,
//             ImageName = "drainage_work.jpg",
//             Date = "2026-06-25"
//         }
//     };

//             return View(mockWorks);
//         }
//     }
// }