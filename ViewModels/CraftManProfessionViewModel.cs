namespace Ostawy.ViewModels;

using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

public class CraftManProfessionViewModel
{
    public Guid ProfessionId { get; set; }

    [Required]
    public string Bio { get; set; } = string.Empty;

    [Range(0,50)]
    public int YearsOfExperience { get; set; }

    public List<IFormFile>? Images { get; set; }
}