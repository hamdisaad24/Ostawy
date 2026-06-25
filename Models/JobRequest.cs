using System;
using System.ComponentModel.DataAnnotations;

namespace Ostawy.Models
{
    public class JobRequest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ClientId { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string Category { get; set; } = string.Empty;

        public decimal? CustomerPrice { get; set; }

        [Required]
        public double Lat { get; set; }

        [Required]
        public double Lng { get; set; }

        public string? Area { get; set; }

        public string? ExactAddress { get; set; }

        public string Status { get; set; } = "Open";

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
