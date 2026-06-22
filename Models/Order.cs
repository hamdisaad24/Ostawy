using System.ComponentModel.DataAnnotations;

namespace Ostawy.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ClientId { get; set; } // إيد العميل اللي طلب

        [Required]
        public int WorkerId { get; set; } // إيد الفني المطلوب

        public string Status { get; set; } = "Pending"; // القيمة الافتراضية: قيد الانتظار

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}