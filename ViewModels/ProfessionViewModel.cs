using System;
using System.ComponentModel.DataAnnotations;

namespace Ostawy.ViewModels;

public class ProfessionViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "اسم الحرفة مطلوب")]
    [Display(Name = "اسم الحرفة")]
    public string Name { get; set; } = string.Empty;
}
