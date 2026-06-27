namespace Ostawy.Models;

public class PasswordResetOtp
{
    public Guid Id { get; set; }

    public string Email { get; set; }

    public string Code { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime ExpireAt { get; set; }

    public bool IsUsed { get; set; }
}
