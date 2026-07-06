using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ostawy.Data;
using Ostawy.Models;
using Ostawy.ViewModels;
using System.Security.Claims;

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

        // عرض صفحة البروفايل الفخمة
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString == null) return RedirectToAction("Login", "Account");

            var user = await _userManager.FindByIdAsync(userIdString);
            if (user == null) return NotFound();

            // جلب بيانات الأوسطى لو موجود
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

                // جلب أسماء الحرف اللي شغالها الأوسطى ده حالياً
                model.CurrentProfessions = await (from cp in _context.Set<CraftsmanProfession>()
                                                  join p in _context.Set<Profession>() on cp.ProfessionId equals p.Id
                                                  where cp.CraftsmanId == craftsman.Id
                                                  select p.Name).ToListAsync();
            }

            // جلب كل الحرف المتاحة في السيستم عشان الـ Modal (Popup)
            model.AllAvailableProfessions = await _context.Set<Profession>().ToListAsync();

            return View(model);
        }

        // ميثود إضافة حرفة جديدة للأوسطى (تشتغل بالـ Modal)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProfession(Guid SelectedProfessionId)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid userId = Guid.Parse(userIdString!);

            // 1. لو مش أوسطى أصلاً.. نكريتله سجل في جدول الـ Craftsman الأول ويتحول لأوسطى!
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

            // 2. تيشك عشان نضمن إنه ما يضيفش نفس الحرفة مرتين
            var alreadyHasIt = await _context.Set<CraftsmanProfession>()
                .AnyAsync(cp => cp.CraftsmanId == craftsman.Id && cp.ProfessionId == SelectedProfessionId);

            if (!alreadyHasIt && SelectedProfessionId != Guid.Empty)
            {
                var newCraftProfession = new CraftsmanProfession
                {
                    CraftsmanId = craftsman.Id,
                    ProfessionId = SelectedProfessionId
                };
                await _context.AddAsync(newCraftProfession);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "يا مسهل الحال! تم إضافة الحرفة بنجاح وبقيت أوسطى رسمي فيها. 🛠️";
            }
            else
            {
                TempData["ErrorMessage"] = "الحرفة دي مسجلة عندك بالفعل يا هندسة!";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}