using Microsoft.AspNetCore.Identity;

namespace Ostawy.Models;

public class ApplicationUser : IdentityUser<Guid>
{
    public string FullName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public double Rating { get; set; }
    public bool IsDeleted { get; set; }
}
