namespace Ostawy.Models;

public class CraftManProfession
{
    public Guid Id { get; set; }
    public Guid CraftsmanId { get; set; }
    public Guid ProfessionId { get; set; }

    public virtual Craftsman? Craftsman { get; set; }
    public virtual Profession? Profession { get; set; }
}