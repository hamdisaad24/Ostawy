using System.Collections.Generic;
using Ostawy.Models;

namespace Ostawy.ViewModels
{
    public class ProfileViewModel
    {
        // بيانات العميل الأساسية
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public double Rating { get; set; }

        // شروط ولوحة الأوسطى
        public bool IsCraftsman { get; set; }
        public string Bio { get; set; }
        public int YearsOfExperience { get; set; }
        public bool IsAvailable { get; set; } // الزرار هيتحكم في دي
        public List<string> CurrentProfessions { get; set; }
    }
}