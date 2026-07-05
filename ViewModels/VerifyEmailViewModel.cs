using System.ComponentModel.DataAnnotations;

namespace Ostawy.ViewModels;

public class VerifyEmailViewModel
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Code { get; set; } = string.Empty;
}
