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
        public required string Title { get; set; }

        [Required]
        public required string Category { get; set; }

        [Required]
        public required string Description { get; set; }

        [Required]
        public required string CoverImagePath { get; set; }

        public List<CraftImage> GalleryImages { get; set; } = new List<CraftImage>();
    }
}
