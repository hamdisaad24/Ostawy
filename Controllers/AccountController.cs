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

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- صفحة الـ REGISTER (إنشاء الحساب) ---

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public IActionResult Register(RegisterViewModel model, string Role)
        {
            if (!ModelState.IsValid) return View(model);

            // 1. التشيك الأول: هل الإيميل ده موجود قبل كده في الداتا بيز؟
            var emailExists = _context.Users.Any(u => u.Email == model.Email);
            if (emailExists)
            {
                // بنضيف خطأ مخصص للإيميل عشان يظهر تحت خانة الإيميل بالظبط
                ModelState.AddModelError("Email", "البريد الإلكتروني هذا مسجل بالفعل لدينا، جرب تسجيل الدخول.");
                return View(model);
            }

            // لو الإيميل مش موجود، بيكمل تسجبل عادي
            var newUser = new User
            {
                FullName = model.FullName,
                Email = model.Email,
                PasswordHash = HashPassword(model.Password),
                Role = Role ?? "client"
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            return RedirectToAction("Login");
        }


        // --- صفحة الـ LOGIN (تسجيل الدخول) ---

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string Role)
        {
            if (!ModelState.IsValid) return View(model);

            string hashedInput = HashPassword(model.Password);

            // 2. التشيك الثاني: هل الإيميل موجود أصلاً؟
            var userEmail = _context.Users.FirstOrDefault(u => u.Email == model.Email);
            if (userEmail == null)
            {
                ModelState.AddModelError(string.Empty, "البريد الإلكتروني غير مسجل لدينا.");
                return View(model);
            }

            // 3. التشيك الثالث: الإيميل موجود بس الباسورد غلط أو الـ Role غلط
            if (userEmail.PasswordHash != hashedInput || userEmail.Role != Role)
            {
                ModelState.AddModelError(string.Empty, "كلمة المرور غير صحيحة، أو نوع الحساب غير متطابق.");
                return View(model);
            }

            // لو كل حاجة صح، يعمل دخول فوراً
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userEmail.FullName),
                new Claim(ClaimTypes.Email, userEmail.Email),
                new Claim(ClaimTypes.Role, userEmail.Role),
                new Claim(ClaimTypes.NameIdentifier, userEmail.Id.ToString()) // مهم جداً عشان الخريطة بعد كده تعرف مين العميل اللي طالب!
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            // التوجيه حسب الدور
            if (userEmail.Role == "worker") return RedirectToAction("Dashboard", "Worker");
            return RedirectToAction("MapView", "Home");
        }

        // دالة التشفير الثابتة
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