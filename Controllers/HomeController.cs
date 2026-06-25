using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ostawy.Data;
using Ostawy.Models;
using System.Security.Claims;

namespace Ostawy.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult WorkerMap()
    {
        return View();
    }

    public IActionResult MapView()
    {
        return View();
    }

    [HttpGet]
    public IActionResult GetWorkersJson()
    {
        var workersList = _context.Users
            .Where(u => u.Role == "worker"
            && u.IsAvailable == true
            && u.Lat != null && u.Lng != null
            && !string.IsNullOrEmpty(u.Specialty)
            && u.Price > 0
            )
            .Select(u => new
            {
                id = u.Id,
                name = u.FullName,
                specialty = u.Specialty,
                category = u.Category,
                rating = u.Rating,
                reviews = u.ReviewsCount,
                price = u.Price,
                lat = u.Lat,
                lng = u.Lng,
                available = u.IsAvailable,
                avatar = string.IsNullOrEmpty(u.FullName) ? "أ" : u.FullName.Substring(0, 1)
            })
            .ToList();

        return Json(workersList);
    }

    public IActionResult Index()
    {
        var clientIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(clientIdStr))
            return RedirectToAction("Login", "Account");

        int clientId = int.Parse(clientIdStr);
        var requests = _context.JobRequests
            .Where(r => r.ClientId == clientId)
            .OrderByDescending(r => r.CreatedAt)
            .ToList();

        var myBids = _context.JobBids
            .Where(b => _context.JobRequests.Any(r => r.Id == b.JobRequestId && r.ClientId == clientId))
            .Join(_context.Users, b => b.WorkerId, u => u.Id, (b, u) => new {
                b.Id,
                b.JobRequestId,
                b.OfferPrice,
                b.Status,
                WorkerName = u.FullName,
                u.Rating,
                u.Specialty
            })
            .Where(b => b.Status == "Pending")
            .ToList();

        ViewBag.Requests = requests;
        ViewBag.MyBids = myBids;
        ViewBag.ClientId = clientId;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateRequest(string description, string category, decimal? customerPrice, double lat, double lng, string area, string exactAddress)
    {
        var clientIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(clientIdStr))
            return Json(new { success = false, message = "يجب تسجيل الدخول أولاً" });

        int clientId = int.Parse(clientIdStr);

        var request = new JobRequest
        {
            ClientId = clientId,
            Description = description,
            Category = category,
            CustomerPrice = customerPrice,
            Lat = lat,
            Lng = lng,
            Area = area,
            ExactAddress = exactAddress,
            Status = "Open",
            CreatedAt = DateTime.Now
        };

        _context.JobRequests.Add(request);
        await _context.SaveChangesAsync();

        return Json(new { success = true, message = "تم نشر طلبك للفنيين بنجاح!", requestId = request.Id });
    }

    [HttpGet]
    public IActionResult RequestDetails(int id)
    {
        var request = _context.JobRequests.Find(id);
        if (request == null) return NotFound();

        var bids = _context.JobBids
            .Where(b => b.JobRequestId == id)
            .Join(_context.Users, b => b.WorkerId, u => u.Id, (b, u) => new
            {
                BidId = b.Id,
                WorkerId = b.WorkerId,
                WorkerName = u.FullName,
                WorkerRating = u.Rating,
                WorkerReviews = u.ReviewsCount,
                WorkerSpecialty = u.Specialty,
                OfferPrice = b.OfferPrice,
                Status = b.Status,
                CreatedAt = b.CreatedAt
            })
            .OrderByDescending(b => b.CreatedAt)
            .ToList();

        ViewBag.Request = request;
        ViewBag.Bids = bids;
        return View();
    }

    [HttpPost]
    public IActionResult AcceptBid(int bidId)
    {
        var bid = _context.JobBids.Find(bidId);
        if (bid == null) return Json(new { success = false, message = "العرض غير موجود" });

        bid.Status = "Accepted";

        var request = _context.JobRequests.Find(bid.JobRequestId);
        if (request != null)
        {
            request.Status = "Closed";
        }

        var otherBids = _context.JobBids
            .Where(b => b.JobRequestId == bid.JobRequestId && b.Id != bidId)
            .ToList();
        foreach (var ob in otherBids) ob.Status = "Rejected";

        _context.SaveChanges();

        return Json(new { success = true, message = "تم قبول العرض! التواصل مع الفني لترتيب الزيارة." });
    }

    [HttpPost]
    public IActionResult CounterOffer(int bidId, decimal newPrice)
    {
        var bid = _context.JobBids.Find(bidId);
        if (bid == null) return Json(new { success = false, message = "العرض غير موجود" });

        bid.OfferPrice = newPrice;
        _context.SaveChanges();

        return Json(new { success = true, message = "تم إرسال السعر الجديد للفني. في انتظار الرد." });
    }

    public IActionResult WorkerProfile(int id)
    {
        var worker = _context.Users.Find(id);
        if (worker == null || worker.Role != "worker") return NotFound();

        var reviews = _context.Reviews
            .Where(r => r.WorkerId == id)
            .OrderByDescending(r => r.CreatedAt)
            .ToList();

        var portfolio = _context.WorkerPortfolios
            .Where(p => p.WorkerId == id)
            .ToList();

        ViewBag.Worker = worker;
        ViewBag.Reviews = reviews;
        ViewBag.Portfolio = portfolio;
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
