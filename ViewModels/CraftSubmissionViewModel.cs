using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Ostawy.ViewModels
{
    public class CraftSubmissionViewModel
    {
        [Required]
        public string CategoryId { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "CoverImage")]
        public IFormFile? CoverImage { get; set; }

        [Display(Name = "GalleryImages")]
        public IFormFile[]? GalleryImages { get; set; }
    }
}
