using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ostawy.Data;
using Ostawy.Models;
using System.Security.Claims;
   using Ostawy.ViewModels; 

namespace Ostawy.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. الأكشن الأساسي اللي هيعرض صفحة الـ Landing Page الجديدة اللي عملناها
        public IActionResult Index()
        {
            return View();
        }

        // 2. الأكشن اللي بيفتح صفحة الخريطة للعميل لتصفح الفنيين
        public IActionResult MapView()
        {
            return View();
        }

        // 3. أكشن إضافي للخريطة لو التيم مستخدمه
        public IActionResult WorkerMap()
        {
            return View();
        }

        // 4. الـ API اللي الـ JavaScript على الخريطة بيكلمه عشان يسحب إحداثيات الفنيين الحقيقية كـ JSON
        [HttpGet]
        public IActionResult GetWorkersJson()
        {
            // عملنا كومنت للشرط مؤقتاً عشان الحقول مش موجودة في كلاس الـ User الحالي عندك
            var workersList = _context.Users
                .Select(u => new
                {
                    id = u.Id,
                    name = u.FullName,
                    specialty = "فني صيانة", // قيمة افتراضية مؤقتة
                    category = "عام",
                    rating = 5.0,
                    reviews = 0,
                    price = 100,
                    lat = 30.0, // إحداثيات مؤقتة
                    lng = 31.0,
                    available = true,
                    avatar = string.IsNullOrEmpty(u.FullName) ? "أ" : u.FullName.Substring(0, 1)
                })
                .ToList();

            return Json(workersList);
        }

        // 5. صفحة سياسة الخصوصية
        public IActionResult Privacy()
        {
            return View();
        }

        // 6. صفحة معالجة الأخطاء الافتراضية للـ .NET
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}