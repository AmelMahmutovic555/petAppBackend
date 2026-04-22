using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Models;

namespace WebApplication1.Dto
{
    public class PetsDto
    {
        public string name { get; set; } = string.Empty;

        public int? age { get; set; }

        public string phone { get; set; }

        public string type { get; set; } = string.Empty;

        public string? image { get; set; }


        //public int? userId { get; set; }

        //[ForeignKey(nameof(userId))]
        //public User? User { get; set; }
    }
}
