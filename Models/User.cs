using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ostawy.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string FullName { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        [Required]
        public string Role { get; set; } = "client";


        public string? Phone { get; set; } // رقم الموبايل للتواصل
        public string? Address { get; set; } // العنوان (مثلاً: الجيزة، الدقي)
        public string? Bio { get; set; } // نبذة عن الفني (مثلاً: خبرة 10 سنوات في تشطيبات السباكة)

        //to map
        public string? Specialty { get; set; }

        public string? Category { get; set; }
        public Double? Lat { get; set; } // خط العرض الجغرافي
        public double? Lng { get; set; } // خط الطول الجغرافي
        public bool IsAvailable { get; set; } = true; // متاح الآن أم مشغول

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; } = 0;


        public int ReviewsCount { get; set; } = 0; // عدد التقييمات
        public double Rating { get; set; } = 5.0; // التقييم من 5

    }
}
