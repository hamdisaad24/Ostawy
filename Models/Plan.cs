namespace Ostawy.Models;

public class Plan
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int MaxRequests { get; set; } 
    public int DurationInDays { get; set; }
    public bool AllowVideos { get; set; }
    public bool HasPrioritySearch { get; set; }
    public bool HasVerifiedBadge { get; set; }
    public bool IsDeleted { get; set; } 
}