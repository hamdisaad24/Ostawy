namespace Ostawy.Models;

public class Payment
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public ApplicationUser? User { get; set; }

    public decimal Amount { get; set; }
    public string Currency { get; set; } = "EGP";
    public DateTime PaymentDate { get; set; }
    public string PaymobOrderId { get; set; } = string.Empty;
    public string PaymobTransactionId { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
}