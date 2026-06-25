namespace Ostawy.Models;

public class EmailVerification
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; }

    public string Code { get; set; }

    public DateTime ExpireAt { get; set; }

    public int NumberOfTry { get; set; }    

    public bool IsUsed { get; set; }
}
