using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ostawy.Models
{
    public class CraftImage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ImagePath { get; set; }

        [ForeignKey("Craft")]
        public int CraftId { get; set; }

        public Craft Craft { get; set; }
    }
}
