using System.Collections.Generic;

namespace Ostawy.ViewModels
{
    public class CraftIndexViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string CoverImagePath { get; set; }
        public List<string> GalleryImagePaths { get; set; } = new List<string>();
    }
}
