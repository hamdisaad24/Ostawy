using System.ComponentModel.DataAnnotations;

namespace Ostawy.Models
{
    public class Category
    {
        [Key] // ده بيميز الجدول بالـ ID
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم القسم مطلوب!")] // عشان ميعديش القسم من غير اسم
        public string Name { get; set; } = string.Empty;

        public string IconPath { get; set; } = string.Empty; // مسار الأيقونة
    }
}