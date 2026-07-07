using System.ComponentModel.DataAnnotations;
using Ostawy.Models;

namespace Ostawy.ViewModels
{
    public class ProfileViewModel
    {
        // بيانات الحساب الأساسية
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public double Rating { get; set; }
        public bool IsCraftsman { get; set; }

        // بيانات الصنايعي (لو هو أوسطى)
        public string Bio { get; set; } = string.Empty;
        public int YearsOfExperience { get; set; }
        public bool IsAvailable { get; set; }

        // قائمة الحرف الحالية اللي عند الأوسطى ده
        public List<string> CurrentProfessions { get; set; } = new List<string>();

        // قائمة كل الحرف المتاحة في السيستم عشان يختار منها لما يدوس "إضافة حرفة"
        public List<Profession> AllAvailableProfessions { get; set; } = new List<Profession>();

        // الـ ID بتاع الحرفة الجديدة اللي هيختارها من الـ Dropdown
        public Guid SelectedProfessionId { get; set; }
    }
}