using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ostawy.Models
{
    public class JobRequest
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }

        [Required]
        public Guid ProfessionId { get; set; }

        [ForeignKey("ProfessionId")]
        public virtual Profession? Profession { get; set; }

        [Required(ErrorMessage = "برجاء كتابة وصف للمشكلة")]
        public string Description { get; set; } = string.Empty; 

        [Required(ErrorMessage = "برجاء تحديد ميزانية تقريبية")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal EstimatedPrice { get; set; } 
        public string Status { get; set; } = "Open"; 
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public virtual ICollection<JobBid>? JobBids { get; set; }   
    }
}