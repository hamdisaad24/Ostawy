using System.Collections.Generic;
using Ostawy.Models;

namespace Ostawy.ViewModels
{
    public class ProfileViewModel
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public double Rating { get; set; }

        public bool IsCraftsman { get; set; }
        public bool IsAvailable { get; set; }
        public List<CraftsmanViewModel> CurrentProfessions { get; set; } = new List<CraftsmanViewModel>();
    }

    public class CraftsmanViewModel
    {
        public Guid Id { get; set; }
        public Guid ProfessionId { get; set; } 
        public string Bio { get; set; } = string.Empty;
        public int YearsOfExperience { get; set; }
        public string Profession { get; set; } = string.Empty;
        public List<string> Images { get; set; } = new();
    }
}