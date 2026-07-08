using System;

namespace Ostawy.Models
{
    public class Profession
    {
        public Guid Id { get; set; }

        // CategoryId links profession to a Category (int). Persisted to DB via migration.
        public int CategoryId { get; set; }

        public string Name { get; set; } = string.Empty;

        // Longer textual description for the profession. Persisted to DB via migration.
        public string Description { get; set; } = string.Empty;
    }
}