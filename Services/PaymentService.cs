using Ostawy.Data;
using Ostawy.Models;

namespace Ostawy.Services;

public class PaymentService
{
    private readonly ApplicationDbContext _context;

    public PaymentService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task CreatePendingPaymentAsync(Guid userId, decimal amount, string paymobOrderId)
    {
        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Amount = amount,
            Currency = "EGP",
            PaymentDate = DateTime.UtcNow,
            PaymobOrderId = paymobOrderId,
            PaymobTransactionId = "PENDING", 
            Status = "Pending" 
        };

        _context.Payments.Add(payment);

        await _context.SaveChangesAsync();
    }
}