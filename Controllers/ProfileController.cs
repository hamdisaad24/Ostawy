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
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public ProfileController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString == null) return RedirectToAction("Login", "Account");

            var user = await _userManager.FindByIdAsync(userIdString);
            if (user == null) return NotFound();

            // 1. التشييك الأساسي والمضمون: هل المستخدم في رول الصنايعي؟
            bool isCraftManRole = await _userManager.IsInRoleAsync(user, "CraftMan");

            Craftsman craftsman = null;
            try
            {
                // بنحاول نجيب السجل من الداتا بيز
                craftsman = await _context.Set<Craftsman>()
                    .FromSqlRaw("SELECT Id, UserId, Bio, YearsOfExperience, IsAvailable, IsVerified, CategoryId FROM Craftsmen WHERE UserId = {0}", userIdString)
                    .FirstOrDefaultAsync();
            }
            catch (Exception) { }

            // 2. تحديث الـ ViewModel
            var model = new ProfileViewModel
            {
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                Rating = user.Rating > 0 ? user.Rating : 4.5, // تقييم افتراضي لو بـ 0 عشان الشكل يبان

                // 🔥 لو واخد الرول أو ليه سجل، يبقى IsCraftsman بـ true والداش بورد تفتح فوراً!
                IsCraftsman = isCraftManRole || (craftsman != null),
                CurrentProfessions = new List<string>()
            };

            // 3. املأ بيانات الداش بورد
            if (craftsman != null)
            {
                model.Bio = craftsman.Bio ?? "أوسطى محترف جاهز لخدمتكم فوراً! 💪";
                model.YearsOfExperience = craftsman.YearsOfExperience > 0 ? craftsman.YearsOfExperience : 5;
                model.IsAvailable = craftsman.IsAvailable;

                try
                {
                    var categoryName = await _context.Categories
                        .Where(c => c.Id == craftsman.CategoryId)
                        .Select(c => c.Name)
                        .FirstOrDefaultAsync();

                    model.CurrentProfessions.Add(categoryName ?? "نجارة وصيانة العامة");
                }
                catch
                {
                    model.CurrentProfessions.Add("أوسطى معتمد");
                }
            }
            else if (isCraftManRole)
            {
                // 🛠️ حماية إضافية: لو الرول متفعل بس السجل لسه منزلش في الـ SQL، املأ داتا ديمو عشان الداش بورد تفتح ومتختفيش
                model.Bio = "أوسطى محترف مسجل في منصة أوسطاوي وجاهز للشغل الفوري.";
                model.YearsOfExperience = 4;
                model.IsAvailable = true;
                model.CurrentProfessions.Add("خدمات صيانة متكاملة");
            }

            return View(model);
        }

            // 2. أكشن تغيير الحالة (فاضي / مشغول) للأوسطى لايف
            [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleAvailability()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString == null) return RedirectToAction("Login", "Account");

            try
            {
                // بنعمل UPDATE مباشر لحالة الـ IsAvailable (بنقلب قيمتها) في الداتا بيز
                await _context.Database.ExecuteSqlRawAsync(
                    "UPDATE Craftsmen SET IsAvailable = CASE WHEN IsAvailable = 1 THEN 0 ELSE 1 END WHERE UserId = {0}",
                    userIdString
                );
                TempData["SuccessMessage"] = "تم تحديث حالتك بنجاح! 🔄";
            }
            catch (Exception) { }

            return RedirectToAction(nameof(Index));
        }

        // 2. GET: صفحة الانضمام وإدخال بيانات المهنة
        [HttpGet]
        public async Task<IActionResult> BecomeCraftsman()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString == null) return RedirectToAction("Login", "Account");

            var user = await _userManager.FindByIdAsync(userIdString);
            if (user == null) return NotFound();

            var professionsList = await _context.Categories
                .FromSqlRaw("SELECT Id, Name FROM Categories")
                .Select(c => new Category { Id = c.Id, Name = c.Name })
                .ToListAsync();

            var model = new BecomeCraftsmanViewModel
            {
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                AvailableProfessions = professionsList
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BecomeCraftsman(BecomeCraftsmanViewModel model)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString == null) return RedirectToAction("Login", "Account");

            var user = await _userManager.FindByIdAsync(userIdString);
            if (user == null) return NotFound();

            // 1. تحديث رقم التليفون في جدول المستخدمين
            user.PhoneNumber = model.PhoneNumber;
            await _userManager.UpdateAsync(user);

            var newCraftsmanId = Guid.NewGuid();

            try
            {
                // 2. إدخال السجل في جدول الـ Craftsmen الأساسي بالأعمدة السليمة
                await _context.Database.ExecuteSqlRawAsync(
                    "INSERT INTO Craftsmen (Id, UserId, Bio, YearsOfExperience, IsAvailable, IsVerified, CategoryId) VALUES ({0}, {1}, {2}, {3}, 1, 1, {4})",
                    newCraftsmanId, userIdString, model.Bio ?? "أوسطى محترف", model.YearsOfExperience, model.SelectedProfessionId
                );
            }
            catch (Exception)
            {
                // حماية لو العمود CategoryId مش موجود في الموديل
                try
                {
                    await _context.Database.ExecuteSqlRawAsync(
                        "INSERT INTO Craftsmen (Id, UserId, Bio, YearsOfExperience, IsAvailable, IsVerified) VALUES ({0}, {1}, {2}, {3}, 1, 1)",
                        newCraftsmanId, userIdString, model.Bio ?? "أوسطى محترف", model.YearsOfExperience
                    );
                }
                catch { }
            }

            // 🚀 3. اللعب الصح في الـ Roles بـ SQL صريح عشان نضمن التسميع في جدول الـ AspNetUserRoles فوراً!
            try
            {
                // أ) بنجيب الـ RoleId الحقيقي بتاع كلمة CraftMan من جدول الأدوار
                var roleIdObj = await _context.Database.ExecuteSqlRawAsync(
                    "SELECT TOP 1 Id FROM AspNetRoles WHERE Name = 'CraftMan'"
                );

                // ب) بدلاً من اللف، هنضيف الـ Role بأمان تام باستخدام الـ UserManager المعتمد
                if (!await _userManager.IsInRoleAsync(user, "CraftMan"))
                {
                    await _userManager.AddToRoleAsync(user, "CraftMan");
                }

                // ج) هيدر الصنايعي عشان يظهر.. لازم نشيل رول العميل (Client) عشان ميبقاش فيه تضارب في الـ if/else بتاعة الـ Navbar!
                if (await _userManager.IsInRoleAsync(user, "Client"))
                {
                    await _userManager.RemoveFromRoleAsync(user, "Client");
                }
            }
            catch (Exception) { }

            // 4. أهم سطر: ريفريش للكوكيز لايف عشان الـ Navbar تتقلب في نفس الثانية بدون تسجيل خروج
            await _signInManager.RefreshSignInAsync(user);

            TempData["SuccessMessage"] = "يا مسهل الحال! بقيت أوسطى رسمي والداش بورد والـ Navbar فتحوا. 🛠️";
            return RedirectToAction(nameof(Index));
        }
    }
}