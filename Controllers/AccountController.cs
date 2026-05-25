using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Ostawy.Data;
using Ostawy.Models;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Ostawy.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        // حقن الداتا بيز جوة الكونترولر (Dependency Injection)
        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login() => View();

        // 1. تسجيل الدخول من الداتا بيز الحقيقية
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string Role)
        {
            if (!ModelState.IsValid) return View(model);

            // تشفير الباسورد اللي المستخدم كتبه عشان نقارنه باللي متخزن مشفر
            string hashedInput = HashPassword(model.Password);

            // البحث عن المستخدم بالإيميل والباسورد المشفر والـ Role
            var user = _context.Users.FirstOrDefault(u => u.Email == model.Email && u.PasswordHash == hashedInput && u.Role == Role);

            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction(user.Role == "worker" ? "WorkerDashboard" : "Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "البيانات غير صحيحة أو الحساب غير موجود بالصلاحية المختارة.");
            return View(model);
        }

        [HttpGet]
        public IActionResult Register() => View();

        // 2. حفظ مستخدم جديد حقيقي في قاعدة البيانات
        [HttpPost]
        public IActionResult Register(RegisterViewModel model, string Role)
        {
            if (!ModelState.IsValid) return View(model);

            // التأكد إن الإيميل مش متكرر قبل كدة
            if (_context.Users.Any(u => u.Email == model.Email))
            {
                ModelState.AddModelError(string.Empty, "هذا البريد الإلكتروني مسجل بالفعل.");
                return View(model);
            }

            // إنشاء كائن المستخدم وحفظ الباسورد مشفر
            var newUser = new User
            {
                FullName = model.FullName,
                Email = model.Email,
                PasswordHash = HashPassword(model.Password), // تشفير قبل الحفظ!
                Role = Role ?? "client"
            };

            _context.Users.Add(newUser); // إضافة للداتا بيز
            _context.SaveChanges(); // حفظ التغييرات حقيقياً

            return RedirectToAction("Login");
        }

        // دالة مساعدة لتشفيير الباسورد بنظام SHA256 لحماية بيانات المستخدمين
        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++) builder.Append(bytes[i].ToString("x2"));
                return builder.ToString();
            }
        }
    }
}