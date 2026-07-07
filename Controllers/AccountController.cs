using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ostawy.Data;
using Ostawy.Interfaces;
using Ostawy.Models;
using Ostawy.ViewModels;
using System.Data;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Ostawy.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailService _emailService;

        public AccountController(ApplicationDbContext context, 
            UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager,
            IEmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var emailExists = await _userManager.FindByEmailAsync(model.Email);
                if (emailExists is not null)
                {
                    ModelState.AddModelError("Email", "البريد الإلكتروني هذا مسجل بالفعل لدينا، جرب تسجيل الدخول.");
                    return View(model);
                }
                var newUser = new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    FullName = model.FullName,
                    Email = model.Email,
                    UserName = model.Email,
                    Address = model.Address,
                    CreatedAt = DateTime.UtcNow,
                };

                IdentityResult result = await _userManager.CreateAsync(newUser, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(newUser, "Client");
                    var otp = Random.Shared.Next(100000, 999999).ToString();

                    _context.EmailVerifications.Add(new EmailVerification
                    {
                        Id = Guid.NewGuid(),
                        UserId = newUser.Id,
                        Code = otp,
                        ExpireAt = DateTime.UtcNow.AddMinutes(5),
                        IsUsed = false
                    });

                    await _context.SaveChangesAsync();

                    await _emailService.SendEmailAsync(
                        newUser.Email!,
                        "رمز التحقق",
                        $"رمز التحقق الخاص بك هو: {otp}");

                    return RedirectToAction(
                        nameof(VerifyEmail),
                        new { userId = newUser.Id , email = newUser.Email });
                }
                else
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult VerifyEmail(Guid userId, string email)
        {
            return View(new VerifyEmailViewModel
            {
                UserId = userId,
                Email = email
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyEmail(VerifyEmailViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.UserId.ToString());
                if (user is null)
                {
                    ModelState.AddModelError("", "يوجد خطأ حاول مرة اخري في وقت لاحق");
                    return View(model);
                }
                if (user.EmailConfirmed)
                {
                    ModelState.AddModelError("", "تم التحقق من الحساب اذهب لتسجيل الدخول");
                }
                var otp = await _context.EmailVerifications
                    .Where(x =>
                        x.UserId == model.UserId &&
                        !x.IsUsed)
                    .OrderByDescending(x => x.ExpireAt)
                    .FirstOrDefaultAsync();
                if (otp is null)
                {
                    ModelState.AddModelError("", "رمز التحقق غير موجود");
                    return View(model);
                }

                if (otp.ExpireAt < DateTime.UtcNow || otp.NumberOfTry >= 5)
                {
                    ModelState.AddModelError("", "انتهت صلاحية الرمز, لقد ارسلنا رمزا آخر");
                    var newOtp = Random.Shared.Next(100000, 999999).ToString();

                    _context.EmailVerifications.Add(new EmailVerification
                    {
                        Id = Guid.NewGuid(),
                        UserId = model.UserId,
                        Code = newOtp,
                        ExpireAt = DateTime.UtcNow.AddMinutes(5),
                        IsUsed = false
                    });

                    await _context.SaveChangesAsync();

                    await _emailService.SendEmailAsync(
                        model.Email!,
                        "رمز التحقق",
                        $"رمز التحقق الخاص بك هو: {newOtp}");
                    return View(model);
                }

                if (otp.Code != model.Code)
                {
                    ModelState.AddModelError("", "رمز التحقق غير صحيح");
                    otp.NumberOfTry += 1;
                    await _context.SaveChangesAsync();
                    return View(model);
                }

                otp.IsUsed = true;

                user!.EmailConfirmed = true;
                var updateResult = await _userManager.UpdateAsync(user);

                if (!updateResult.Succeeded)
                {
                    ModelState.AddModelError("", "حدث خطأ أثناء تفعيل الحساب");
                    return View(model);
                }

                await _context.SaveChangesAsync();

                await _signInManager.SignInAsync(user, false);

                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendOtp(Guid userId, string email)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null)
                return RedirectToAction(nameof(Register));

            var otp = Random.Shared.Next(100000, 999999).ToString();

            _context.EmailVerifications.Add(new EmailVerification
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Code = otp,
                ExpireAt = DateTime.UtcNow.AddMinutes(5),
                IsUsed = false
            });

            await _context.SaveChangesAsync();

            await _emailService.SendEmailAsync(
                email,
                "رمز التحقق",
                $"رمز التحقق الخاص بك هو: {otp}");

            TempData["SuccessMessage"] =
                "تم إرسال رمز تحقق جديد إلى بريدك الإلكتروني";

            return RedirectToAction(
                nameof(VerifyEmail),
                new
                {
                    userId,
                    email
                });
        }


        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "البريد الإلكتروني أو كلمة المرور غير صحيحة.");
                return View(model);
            }

            if (!user.EmailConfirmed)
            {
                ModelState.AddModelError("", "يجب تأكيد البريد الإلكتروني أولاً.");
                return View(model);
            }

            bool passwordMatched = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!passwordMatched)
            {
                ModelState.AddModelError("", "يوجد خطأ حاول في وقت لاحق.");
                return View(model);
            }
            
            
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            await _signInManager.SignInWithClaimsAsync(
                user,
                isPersistent: model.RememberMe,
                claims
            );

            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "الإيميل غير صحيح");
                return View(model);
            }

            var otp = new Random().Next(100000, 999999).ToString();

            var oldOtps = _context.PasswordResetOtps
                    .Where(x => x.Email == model.Email && !x.IsUsed);

            _context.PasswordResetOtps.RemoveRange(oldOtps);

            var newOtp = new PasswordResetOtp
            {
                Id = Guid.NewGuid(),
                Email = model.Email,
                Code = otp,
                CreatedAt = DateTime.UtcNow,
                ExpireAt = DateTime.UtcNow.AddMinutes(5),
                IsUsed = false
            };

            _context.PasswordResetOtps.Add(newOtp);
            await _context.SaveChangesAsync();

            await _emailService.SendEmailAsync(model.Email, "Reset OTP", $"Your OTP: {otp}");

            return RedirectToAction("VerifyOtp", new { email = model.Email });
        }


        [HttpGet]
        public IActionResult VerifyOtp(string email)
        {
            var model = new VerifyOtpViewModel
            {
                Email = email
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyOtp(VerifyOtpViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var otpRecord = await _context.PasswordResetOtps
                .FirstOrDefaultAsync(x =>
                    x.Email == model.Email &&
                    x.Code == model.Code &&
                    !x.IsUsed);

            if (otpRecord == null)
            {
                ModelState.AddModelError("", "الكود غير صحيح");
                return View(model);
            }

            if (otpRecord.ExpireAt < DateTime.UtcNow)
            {
                ModelState.AddModelError("", "الكود انتهت صلاحيته");
                return View(model);
            }

            // mark OTP as used
            otpRecord.IsUsed = true;
            _context.PasswordResetOtps.Update(otpRecord);
            await _context.SaveChangesAsync();

            // مهم: تمرير الإيميل لخطوة reset password
            return RedirectToAction("ResetPassword", new { email = model.Email });
        }

        [HttpGet]
        public IActionResult ResetPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("ForgotPassword");

            return View(new ResetPasswordViewModel
            {
                Email = email
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "المستخدم غير موجود");
                return View(model);
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

                return View(model);
            }

            // optional: clean all OTPs for that email
            var otps = _context.PasswordResetOtps
                .Where(x => x.Email == model.Email);

            _context.PasswordResetOtps.RemoveRange(otps);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login");
        }

        [HttpGet]
        [Authorize]
        public IActionResult ChangePassword()
        {
            return View(new ChangePasswordViewModel
            {
                Email = User.FindFirstValue(ClaimTypes.Email)!
            });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "المستخدم غير موجود");
                return View(model);
            }

            var result = await _userManager.ChangePasswordAsync(
                user,
                model.CurrentPassword,
                model.NewPassword
            );

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

                return View(model);
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}