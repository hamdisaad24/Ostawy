namespace Ostawy.Models;

public class CraftManProfessionImage
{
    public Guid Id { get; set; }

    public Guid CraftManProfessionId { get; set; }

    public string ImagePath { get; set; } = string.Empty;

    public virtual CraftManProfession? CraftManProfession { get; set; }
}