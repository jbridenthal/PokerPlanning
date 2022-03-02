using static PokerPlanning.Shared.Enums;

namespace PokerPlanning.Shared
{
    public class User
    {
        public string? ID { get; set; } 
        public string? Name { get; set; }
        public Role Role { get; set; }
        public string? Vote { get; set; }
        public string? Room { get; set; }
    }
}
