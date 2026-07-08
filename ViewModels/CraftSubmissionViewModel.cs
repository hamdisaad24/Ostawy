using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Ostawy.ViewModels
{
    public class CraftSubmissionViewModel
    {
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Category is required.")]
        public string CategoryId { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Required(ErrorMessage = "A cover image is required.")]
        public IFormFile CoverImage { get; set; }

        public List<IFormFile> GalleryImages { get; set; } = new List<IFormFile>();
    }
}
