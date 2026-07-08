using System.ComponentModel.DataAnnotations.Schema;

namespace Ostawy.Models
{
    public class Craftsman
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; } = new ApplicationUser();
        public string Bio { get; set; } = string.Empty;
        public int YearsOfExperience { get; set; }
        public bool IsVerified { get; set; }
        public bool IsAvailable { get; set; }
        public int CategoryId { get; internal set; }
    }
}
