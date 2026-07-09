using System.Collections.Generic;
using Ostawy.Models;

namespace Ostawy.ViewModels
{
    public class ProfileViewModel
    {
        // بيانات العميل الأساسية
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public double Rating { get; set; }

        // شروط ولوحة الأوسطى
        public bool IsCraftsman { get; set; }
        public string Bio { get; set; } = string.Empty;
        public int YearsOfExperience { get; set; }
        public bool IsAvailable { get; set; } // الزرار هيتحكم في دي
        public List<string> CurrentProfessions { get; set; } = new List<string>();
    }
}