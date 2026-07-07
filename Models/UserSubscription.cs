namespace Ostawy.Models;

public class UserSubscription
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public ApplicationUser? User { get; set; }
    
    public Guid PlanId { get; set; }
    public Plan? Plan { get; set; }
    
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; }
    public string LatestPaymobOrderId { get; set; } = string.Empty;
}