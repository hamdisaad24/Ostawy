using Microsoft.AspNetCore.Identity;

namespace Ostawy.Models;

public class ApplicationUser : IdentityUser<Guid>
{
    public string FullName { get; set; }
    public string Address { get; set; } 
    public DateTime CreatedAt { get; set; }
    public double Rating { get; set; }
}
