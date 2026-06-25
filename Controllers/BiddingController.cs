using Microsoft.AspNetCore.Mvc;
using Ostawy.Data;
using Ostawy.Models;
using System;
using System.Linq;

namespace Ostawy.Controllers
{
    public class BiddingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BiddingController(ApplicationDbContext context)
        {
            _context = context;
        }

        //// 1. العميل بينشر طلب جديد على الخريطة
        //[HttpPost]
        //public IActionResult CreateJobRequest([FromBody] JobRequest request)
        //{
        //    if (request == null) return BadRequest("بيانات الطلب غير صالحة");

        //    request.Status = "Open";
        //    request.CreatedAt = DateTime.Now;

        //    _context.JobRequests.Add(request);
        //    _context.SaveChanges();

        //    return Json(new { success = true, requestId = request.Id, message = "تم نشر طلبك للفنيين بنجاح! 🚀" });
        //}

        //// 2. الفني بيسحب الطلبات المفتوحة القريبة منه عشان تظهر على خريطته
        //[HttpGet]
        //public IActionResult GetActiveRequestsJson(string category = "all")
        //{
        //    var requests = _context.JobRequests
        //        .Where(r => r.Status == "Open")
        //        .AsQueryable();

        //    // لو الفني محدد قسم معين (سباكة مثلاً) يظهرله طلبات السباكة بس
        //    if (category != "all")
        //    {
        //        requests = requests.Where(r => r.Category == category);
        //    }

        //    var result = requests.Select(r => new
        //    {
        //        id = r.Id,
        //        clientId = r.ClientId,
        //        description = r.Description,
        //        category = r.Category,
        //        customerPrice = r.CustomerPrice,
        //        lat = r.Lat,
        //        lng = r.Lng,
        //        createdAt = r.CreatedAt
        //    }).ToList();


        //    return Json(result);
        //}

        //// 3. الفني بيقدم عرض سعر على طلب العميل
        //[HttpPost]
        //public IActionResult AddBid(int requestId, int workerId, decimal offerPrice)
        //{
        //    // التأكد إن الطلب لسه مفتوح ومقفلش
        //    var request = _context.JobRequests.Find(requestId);
        //    if (request == null || request.Status != "Open")
        //        return BadRequest("هذا الطلب لم يعد متاحاً متاحاً.");

        //    var newBid = new JobBid
        //    {
        //        JobRequestId = requestId,
        //        WorkerId = workerId,
        //        OfferPrice = offerPrice,
        //        Status = "Pending" // في انتظار موافقة العميل
        //    };

        //    _context.JobBids.Add(newBid);
        //    _context.SaveChanges();

        //    return Json(new { success = true, message = "تم إرسال عرض السعر الخاص بك للعميل! 💰" });
        //}

        //// 4. العميل بيوافق على عرض فني معين من العروض اللي جتلُه
        //[HttpPost]
        //public IActionResult AcceptBid(int bidId)
        //{
        //    var bid = _context.JobBids.Find(bidId);
        //    if (bid == null) return NotFound("العرض غير موجود");

        //    // 1. تحديث حالة العرض المختار وموافقته
        //    bid.Status = "Accepted";

        //    // 2. قفل طلب العميل عشان مفيش فني تاني يقدم عليه
        //    var request = _context.JobRequests.Find(bid.JobRequestId);
        //    if (request != null)
        //    {
        //        request.Status = "Closed";
        //    }

        //    // 3. رفض باقي العروض اللي كانت مقدمة على نفس الطلب أوتوماتيكياً
        //    var otherBids = _context.JobBids
        //        .Where(b => b.JobRequestId == bid.JobRequestId && b.Id != bidId)
        //        .ToList();

        //    foreach (var otherBid in otherBids)
        //    {
        //        otherBid.Status = "Rejected";
        //    }

        //    _context.SaveChanges();

        //    return Json(new { success = true, message = "تم قبول العرض! اطلب من الأوستا التوجه إليك. 🛠️" });
        //}

        //// اكشن إضافي: العميل بيشوف العروض اللي جت على طلبه الحالي
        //[HttpGet]
        //public IActionResult GetBidsForRequest(int requestId)
        //{
        //    var bids = _context.JobBids
        //        .Where(b => b.JobRequestId == requestId && b.Status == "Pending")
        //        .Select(b => new
        //        {
        //            bidId = b.Id,
        //            workerId = b.WorkerId,
        //            offerPrice = b.OfferPrice,
        //            // بنجيب بيانات الفني من جدول الـ Users عشان نعرض اسمه وتقييمه للعميل
        //            workerName = _context.Users.Where(u => u.Id == b.WorkerId).Select(u => u.FullName).FirstOrDefault(),
        //            workerRating = _context.Users.Where(u => u.Id == b.WorkerId).Select(u => u.Rating).FirstOrDefault()
        //        }).ToList();

        //    return Json(bids);
        //}
    }
}