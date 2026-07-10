using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ostawy.Models
{
    public class JobBid
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid JobRequestId { get; set; }

        [ForeignKey("JobRequestId")]
        public JobRequest? JobRequest { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "برجاء تحديد عرض السعر الخاص بك")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal OfferPrice { get; set; }
        public string? Note { get; set; }
        public string Status { get; set; } = "Pending"; 
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}