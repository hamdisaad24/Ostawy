using System.ComponentModel.DataAnnotations.Schema;

namespace Ostawy.Models
{
    public class Craftsman
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        public bool IsAvailable { get; set; }   

        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }
    }
}
