using System;
using System.ComponentModel.DataAnnotations;

namespace Ostawy.Models
{
    public class JobBid
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int JobRequestId { get; set; } // مربوط برقم طلب العميل

        [Required]
        public int WorkerId { get; set; } // رقم الفني اللي مقدم العرض

        [Required]
        public decimal OfferPrice { get; set; } // السعر اللي الفني عارضه (ممكن يزود أو يقلل عن سعر العميل)

        public string Status { get; set; } = "Pending"; // الحالة: Pending (انتظار رد العميل)، Accepted (مقبول)، Rejected (مرفوض)

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}