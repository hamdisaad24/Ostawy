using System;

namespace Ostawy.Models
{
    public class Profession
    {
        public Guid Id { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}