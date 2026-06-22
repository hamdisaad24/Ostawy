using System;
using System.ComponentModel.DataAnnotations;

namespace Ostawy.Models
{
    public class JobRequest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ClientId { get; set; } // رقم العميل اللي عمل الطلب

        [Required]
        [StringLength(500)]
        public string Description { get; set; } // تفاصيل المشكلة (مثلاً: ماسورة مكسورة في الحمام)

        [Required]
        public string Category { get; set; } // القسم (plumbing, electric, ac, paint)

        [Required]
        public decimal CustomerPrice { get; set; } // السعر الافتراضي اللي العميل عرضه

        [Required]
        public double Lat { get; set; } // خط العرض لمكان المشكلة

        [Required]
        public double Lng { get; set; } // خط الطول لمكان المشكلة

        public string Status { get; set; } = "Open"; // الحالة: Open (مفتوح للعروض) أو Closed (تم الاختيار والانتهاء)

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}