using System.ComponentModel.DataAnnotations;

namespace Ostawy.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int WorkerId { get; set; }

        [Required]
        public int ClientId { get; set; }

        [Required]
        public int Rating { get; set; }

        [StringLength(500)]
        public string? Comment { get; set; }

        public string? ClientName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
