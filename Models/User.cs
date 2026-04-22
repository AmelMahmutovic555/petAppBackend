namespace WebApplication1.Models
{
    public class User
    {
        public int id { get; set; }
        public string name { get; set; } = string.Empty;

        public string surname { get; set; } = string.Empty;

        public string email { get; set; } = string.Empty;

        public string password { get; set; } = string.Empty;

        public string phone { get; set; } = string.Empty;
        public string role { get; set; } = "User";

        public string? GoogleId { get; set; }
        public string? AuthProvider { get; set; }

        public ICollection<Pets> pets { get; set; } = new List<Pets>();
    }
}
