using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class Pets
    {
        public int id { get; set; }
        public string name { get; set; } = string.Empty;

        public int? age { get; set; }

        public string phone { get; set; }

        public string type { get; set; } = string.Empty;

        public string? image { get; set; }

        public int? userId { get; set; }

        [ForeignKey(nameof(userId))]
        public User? User { get; set; }

    }
}
