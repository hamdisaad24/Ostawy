using System;
using System.Collections.Generic;
using Ostawy.Models;

namespace Ostawy.ViewModels
{
    public class ProfileViewModel
    {
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public double Rating { get; set; }
        public bool IsCraftsman { get; set; }

        public string Bio { get; set; } = string.Empty;
        public int YearsOfExperience { get; set; }
        public bool IsAvailable { get; set; }

        public List<string> CurrentProfessions { get; set; } = new List<string>();

        // 🛠️ تم تعديل النوع هنا ليقرأ من كلاس الـ Category الجديد بتاعك
        public List<Category> AllAvailableProfessions { get; set; } = new List<Category>();

        // 🛠️ تم تعديل الـ ID ليكون int متوافق مع جدول الـ Categories الموحد
        public int SelectedProfessionId { get; set; }
    }
}