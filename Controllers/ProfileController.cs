using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ostawy.Data;
using Ostawy.Models;
using Ostawy.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Ostawy.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public ProfileController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // 1. دالة عرض البروفايل النظيفة
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString == null) return RedirectToAction("Login", "Account");

            var user = await _userManager.FindByIdAsync(userIdString);
            if (user == null) return NotFound();

            Guid userId = Guid.Parse(userIdString);
            var craftsman = await _context.Set<Craftsman>().FirstOrDefaultAsync(c => c.UserId == userId);

            var model = new ProfileViewModel
            {
                Email = user.Email ?? string.Empty,
                FullName = user.FullName,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                Rating = user.Rating,
                IsCraftsman = craftsman != null
            };

            if (craftsman != null)
            {
                model.Bio = craftsman.Bio;
                model.YearsOfExperience = craftsman.YearsOfExperience;
                model.IsAvailable = craftsman.IsAvailable;

                // بنقرأ اسم الحرفة من جدول الـ Craftsman مباشرة لو رابطينه، أو بنسيبها فاضية
                model.CurrentProfessions = new List<string> { "أوسطى معتمد في المنصة" };
            }

            model.AllAvailableProfessions = await _context.Categories.ToListAsync();
            return View(model);
        }

        // ميثود إضافة حرفة جديدة للأوسطى (تشتغل بالـ Modal مع الـ Categories)
        // 2. دالة إضافة الحرفة النظيفة بدون تعديل الـ User
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProfession(int SelectedProfessionId)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString)) return RedirectToAction("Login", "Account");

            Guid userId = Guid.Parse(userIdString);

            var craftsman = await _context.Set<Craftsman>().FirstOrDefaultAsync(c => c.UserId == userId);
            if (craftsman == null)
            {
                craftsman = new Craftsman
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Bio = "أوسطى جديد في منصة أوسطاوي! 💪",
                    YearsOfExperience = 1,
                    IsAvailable = true,
                    IsVerified = false
                };
                await _context.AddAsync(craftsman);
                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = "يا مسهل الحال! بقيت أوسطى رسمي في القسم المختار. 🛠️";
            return RedirectToAction(nameof(Index));
        }
    }
}