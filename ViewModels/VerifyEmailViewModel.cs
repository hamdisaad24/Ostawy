using System.ComponentModel.DataAnnotations;

namespace Ostawy.ViewModels;

public class VerifyEmailViewModel
{
    public Guid UserId { get; set; }
    public string Email { get; set; }

    [Required]
    public string Code { get; set; }
}
