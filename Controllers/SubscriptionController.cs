using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Ostawy.Data;
using Ostawy.Helpers;
using Ostawy.Services;
using Microsoft.EntityFrameworkCore;
using Ostawy.Models;

namespace Ostawy.Controllers;

[Authorize]
public class SubscriptionController : Controller
{
    private readonly PaymobSettings _paymobSettings;
    private readonly PaymentService _paymentService;
    private readonly ApplicationDbContext _context;
    private readonly HttpClient _httpClient;

    public SubscriptionController(IOptions<PaymobSettings> options, PaymentService paymentService, ApplicationDbContext context)
    {
        _paymobSettings = options.Value;
        _paymentService = paymentService;
        _context = context;
        _httpClient = new HttpClient();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpgradeToPro(Guid planId)
    {
        var plan = await _context.Plans.FindAsync(planId);
        if (plan == null)
        {
            return NotFound("الخطة المطلوبة غير موجودة.");
        }
        
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdString, out Guid userId))
        {
            return Unauthorized();
        }

        try
        {
            var authUrl = "https://accept.paymob.com/api/auth/tokens";
            var authData = new { api_key = _paymobSettings.APIKEY };
            
            var authResponse = await _httpClient.PostAsync(authUrl, 
                new StringContent(JsonSerializer.Serialize(authData), Encoding.UTF8, "application/json"));
            
            if (!authResponse.IsSuccessStatusCode) return BadRequest("فشل الاتصال الأولي ببوابة الدفع.");
            
            var authResult = JsonSerializer.Deserialize<JsonElement>(await authResponse.Content.ReadAsStringAsync());
            string token = authResult.GetProperty("token").GetString()!;

            var orderUrl = "https://accept.paymob.com/api/ecommerce/orders";
            var orderData = new
            {
                auth_token = token,
                delivery_needed = "false",
                amount_cents = (int)(plan.Price * 100),
                currency = "EGP",
                items = new[] 
                { 
                    new { name = $"{plan.Name} Plan", amount_cents = (int)(plan.Price * 100), quantity = 1 } 
                }
            };

            var orderResponse = await _httpClient.PostAsync(orderUrl, 
                new StringContent(JsonSerializer.Serialize(orderData), Encoding.UTF8, "application/json"));
            
            if (!orderResponse.IsSuccessStatusCode) return BadRequest("فشل تسجيل طلب الدفع.");

            var orderResult = JsonSerializer.Deserialize<JsonElement>(await orderResponse.Content.ReadAsStringAsync());
            string paymobOrderId = orderResult.GetProperty("id").GetInt64().ToString();

            await _paymentService.CreatePendingPaymentAsync(userId, plan.Id, plan.Price, paymobOrderId);

            var paymentKeyUrl = "https://accept.paymob.com/api/acceptance/payment_keys"; 
            var paymentKeyData = new
            {
                auth_token = token,
                amount_cents = (int)(plan.Price * 100),
                expiration = 3600,
                order_id = paymobOrderId,
                billing_data = new
                {
                    apartment = "NA", floor = "NA", building = "NA", street = "NA", postal_code = "NA", city = "NA", country = "NA",
                    first_name = User.Identity?.Name ?? "Ostawy",
                    last_name = "Craftsman",
                    email = User.FindFirstValue(ClaimTypes.Email) ?? "craftsman@ostawy.com",
                    phone_number = "+201000000000"
                },
                currency = "EGP",
                integration_id = int.Parse(_paymobSettings.INTEGRATIONID)
            };

            var paymentKeyResponse = await _httpClient.PostAsync(paymentKeyUrl, 
                new StringContent(JsonSerializer.Serialize(paymentKeyData), Encoding.UTF8, "application/json"));
            
            if (!paymentKeyResponse.IsSuccessStatusCode) return BadRequest("فشل توليد مفتاح الدفع.");

            var paymentKeyResult = JsonSerializer.Deserialize<JsonElement>(await paymentKeyResponse.Content.ReadAsStringAsync());
            string paymentToken = paymentKeyResult.GetProperty("token").GetString()!;

            string iframeId = _paymobSettings.IFRAMEID;
            string paymentRedirectUrl = $"https://accept.paymob.com/api/acceptance/iframes/{iframeId}?payment_token={paymentToken}";

            return Redirect(paymentRedirectUrl);
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "عذراً، حدث خطأ غير متوقع أثناء إعداد عملية الدعم. حاول مرة أخرى.";
            return RedirectToAction("Index", "Home");
        }
    }


    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Callback()
    {
        var success = Request.Query["success"].ToString();
        var orderId = Request.Query["order"].ToString();
        var payment = await _context.Payments
            .FirstOrDefaultAsync(x => x.PaymobOrderId == orderId);

        if(payment == null)
            return NotFound();

        if(success != "true")
        {
            payment.Status = "Failed";
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Cancel));
        }

        payment.Status = "Paid";
        payment.PaymobTransactionId =
            Request.Query["id"].ToString();

        var subscription = new UserSubscription
        {
            Id = Guid.NewGuid(),
            UserId = payment.UserId,
            PlanId = payment.PlanId,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(30),
            IsActive = true,
            LatestPaymobOrderId = payment.PaymobOrderId
        };

        _context.UserSubscriptions.Add(subscription);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Success));
    }

    [HttpGet]
    public IActionResult Success()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Cancel()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Upgrade()
    {
        var plans = await _context.Plans.Where(p => !p.IsDeleted).ToListAsync();
        return View(plans);
    }
}