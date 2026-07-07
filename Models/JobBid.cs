using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ostawy.Models
{
    public class JobBid
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int JobRequestId { get; set; } // مرتبط بـ أنهي طلب صيانة

        [ForeignKey("JobRequestId")]
        public virtual JobRequest JobRequest { get; set; }

        [Required]
        public string ArtisanId { get; set; } // معرف الصنايعي اللي قدم العرض

        [Required(ErrorMessage = "برجاء تحديد عرض السعر الخاص بك")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal OfferPrice { get; set; } // السعر اللي الصنايعي عارضه

        public string Note { get; set; } // رسالة أو ملاحظة من الصنايعي للعميل (مثلاً: هجيلك بكرا الصبح)

        public string Status { get; set; } = "Pending"; // حالة العرض (Pending, Approved, Rejected)

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}