using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

            bool isCraftManRole = await _userManager.IsInRoleAsync(user, "CraftMan");
            var model = new ProfileViewModel
            {
                FullName = user.FullName,
                Email = user.Email ?? "No Email",
                Address = user.Address ?? "No Address",
                PhoneNumber = user.PhoneNumber ?? "No Phone Number",
                Rating = user.Rating > 0 ? user.Rating : 4.5,
                IsCraftsman = isCraftManRole
            };

            if (isCraftManRole)
            {
                var craftsman = await _context.Craftsmen.FirstOrDefaultAsync(c => c.UserId == user.Id);
                
                var professions = await _context.CraftManProfessions
                            .Include(x => x.Profession)
                            .Include(x => x.Images)
                            .Where(x => x.CraftsmanId == craftsman!.Id)
                            .ToListAsync();
                
                model.IsAvailable = craftsman!.IsAvailable;
                model.CurrentProfessions = new List<CraftsmanViewModel>();
                foreach (var profession in professions)
                {
                    model.CurrentProfessions.Add(new CraftsmanViewModel
                    {
                        Id = profession.Id,
                        Bio = profession.Bio,
                        YearsOfExperience = profession.YearsOfExperience,
                        ProfessionId = profession.Profession!.Id,
                        Profession = profession.Profession!.Name,

                        Images = profession!.Images!
                                    .Select(x => x.ImagePath)
                                    .ToList()
                    });
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleAvailability()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString == null) return RedirectToAction("Login", "Account");

            try
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "UPDATE Craftsmen SET IsAvailable = CASE WHEN IsAvailable = 1 THEN 0 ELSE 1 END WHERE UserId = {0}",
                    userIdString
                );
                TempData["SuccessMessage"] = "تم تحديث حالتك بنجاح! 🔄";
            }
            catch (Exception) { }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> BecomeCraftsman()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString == null) return RedirectToAction("Login", "Account");

            var user = await _userManager.FindByIdAsync(userIdString);
            if (user == null) return NotFound();
            ViewBag.Professions = new SelectList(
                    await _context.Professions.ToListAsync(),
                    "Id",
                    "Name");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BecomeCraftsman(CraftManProfessionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Professions = new SelectList(
                    await _context.Professions.ToListAsync(),
                    "Id",
                    "Name");

                return View(model);
            }

            var userId = Guid.Parse(_userManager.GetUserId(User)!);
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return NotFound();

            await _userManager.AddToRoleAsync(user, "CraftMan");

            var craftsman = await _context.Craftsmen
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (craftsman == null)
            {
                craftsman = new Craftsman
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    IsAvailable = true
                };

                _context.Craftsmen.Add(craftsman);
            }

            bool exists = await _context.CraftManProfessions.AnyAsync(x =>
                x.CraftsmanId == craftsman.Id &&
                x.ProfessionId == model.ProfessionId);

            if (exists)
            {
                ModelState.AddModelError("", "لقد قمت بإضافة هذه الحرفة بالفعل.");

                ViewBag.Professions = new SelectList(
                    await _context.Professions.ToListAsync(),
                    "Id",
                    "Name");

                return View(model);
            }

            var craftProfession = new CraftManProfession
            {
                Id = Guid.NewGuid(),
                CraftsmanId = craftsman.Id,
                ProfessionId = model.ProfessionId,
                Bio = model.Bio,
                YearsOfExperience = model.YearsOfExperience,
                IsVerified = false
            };

            _context.CraftManProfessions.Add(craftProfession);

            if (model.Images != null && model.Images.Any())
            {
                var folder = Path.Combine(
                    "wwwroot",
                    "images");

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                foreach (var image in model.Images.Take(5))
                {
                    var fileName =
                        $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";

                    var filePath = Path.Combine(folder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    _context.CraftManProfessionImages.Add(
                        new CraftManProfessionImage
                        {
                            Id = Guid.NewGuid(),
                            CraftManProfessionId = craftProfession.Id,
                            ImagePath = "/images/" + fileName
                        });
                }
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "تم إضافة الحرفة بنجاح، وسيتم مراجعتها من الإدارة.";

            return RedirectToAction(nameof(Index));
        }
    }
}