using Microsoft.AspNetCore.Identity;

namespace PokerPlanning.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        private string? lastName;
        private string? firstName;
        private int? defaultRoom;

        public string FirstName { get => firstName ?? string.Empty; set => firstName = value; }
        public string LastName { get => lastName ?? string.Empty; set => lastName = value; }
        public int DefaultRoom { get => defaultRoom ?? 0; set => defaultRoom = value; }
    }
}
