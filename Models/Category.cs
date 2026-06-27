using System.ComponentModel.DataAnnotations;

namespace Ostawy.Models
{
    public class Category
    {
        [Key] // ده بيميز الجدول بالـ ID
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم القسم مطلوب!")] // عشان ميعديش القسم من غير اسم
        public string Name { get; set; }

        public string IconPath { get; set; } // مسار الأيقونة
    }
}