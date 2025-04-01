
namespace PokerPlanning.Models
{
    public class User
    {
        public string? ID { get; set; }
        public string? Name { get; set; }
        public Role Role { get; set; }
        public string? Vote { get; set; }
        public string? Room { get; set; }
        public string PasswordHash { get; set; }
        public string Salt { get; set; }
        
    }
}
