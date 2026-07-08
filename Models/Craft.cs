using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ostawy.Models
{
    public class Craft
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [Required]
        public string Category { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string CoverImagePath { get; set; }

        public List<CraftImage> GalleryImages { get; set; } = new List<CraftImage>();
    }
}
