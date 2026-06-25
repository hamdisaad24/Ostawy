using System.ComponentModel.DataAnnotations;

namespace Ostawy.Models
{
    public class WorkerPortfolio
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int WorkerId { get; set; }

        [Required]
        public string ImageUrl { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
