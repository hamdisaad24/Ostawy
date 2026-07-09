using Microsoft.AspNetCore.Identity;

namespace Ostawy.Models;

public class ApplicationUser : IdentityUser<Guid>
{
    public string FullName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public double Rating { get; set; }
<<<<<<< HEAD
    public bool IsDeleted { get; set; }
=======

    //public string Specialty { get; set; } = string.Empty;
>>>>>>> 620284492b73bc8bc050667f04e3aa68c3d63443
}
