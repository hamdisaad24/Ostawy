using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ostawy.Models
{
    public class JobRequest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? ClientId { get; set; } // المعرف الخاص بالعميل اللي نشر الطلب

        [Required]
        public int CategoryId { get; set; } // ربط مع جدول الأقسام (سباك، نجار..) اللي جاية من الداتا بيز

        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }

        [Required(ErrorMessage = "برجاء كتابة وصف للمشكلة")]
        public string Description { get; set; } // وصف المشكلة

        [Required(ErrorMessage = "برجاء تحديد ميزانية تقريبية")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal EstimatedPrice { get; set; } // السعر التقريبي

        public string Status { get; set; } = "Open"; // حالة الطلب (Open, Accepted, Closed)

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // علاقة مع العروض المقدمة على هذا الطلب
        public virtual ICollection<JobBid> JobBids { get; set; } = new List<JobBid>();
    }
}