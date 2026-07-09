using System.Collections.Generic;
using Ostawy.Models;

namespace Ostawy.ViewModels
{
    public class BecomeCraftsmanViewModel
    {
        public string PhoneNumber { get; set; }
        public int SelectedProfessionId { get; set; }
        public int YearsOfExperience { get; set; } // الحقل الجديد
        public string Bio { get; set; } // الحقل الجديد
        public List<Category> AvailableProfessions { get; set; }
    }
}