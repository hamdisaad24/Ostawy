using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Ostawy.Models;
using Ostawy.Data;
using System.Linq;


namespace Ostawy.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }
    //// 1. الأكشن اللي بيفتح صفحة الخريطة
    //public IActionResult MapView()
    //{
    //    return View();
    //}

    //// 2. الـ API اللي الـ JavaScript هيكلمه عشان ياخد بيانات الفنيين الحقيقية
    //[HttpGet]
    //public IActionResult GetWorkersJson()
    //{
    //    // سحب المستخدمين اللي الـ Role بتاعهم worker فقط من الداتا بيز
    //    var workersList = _context.Users
    //        .Where(u => u.Role == "worker"
    //        && u.IsAvailable == true
    //        && u.Lat != null && u.Lng != null
    //        && !string.IsNullOrEmpty(u.Specialty)
    //        && u.Price > 0
    //        )
    //        .Select(u => new
    //        {
    //            id = u.Id,
    //            name = u.FullName,
    //            specialty = u.Specialty,
    //            category = u.Category,
    //            rating = u.Rating,
    //            reviews = u.ReviewsCount,
    //            price = u.Price,
    //            lat = u.Lat,
    //            lng = u.Lng,
    //            available = u.IsAvailable,
    //            avatar = string.IsNullOrEmpty(u.FullName) ? "أ" : u.FullName.Substring(0, 1) // بيجيب أول حرف للافتار
    //        })
    //        .ToList();

    //    return Json(workersList); // بيرجع البيانات كـ JSON
    //}


    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    //public IActionResult Error()
    //{
    //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    //}

    //public IActionResult WorkerDashboard()
    //{
    //    return Content("أهلاً بك في لوحة تحكم الفني (تحت الإنشاء)");
    //}
}
