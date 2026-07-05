namespace Ostawy.Models;

public class PasswordResetOtp
{
    public Guid Id { get; set; }

    public string Email { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime ExpireAt { get; set; }

    public bool IsUsed { get; set; }
}
