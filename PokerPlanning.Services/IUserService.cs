
namespace PokerPlanning.Services
{
    public interface IUserService
    {
        Task<User> GetUserAsync(string username);
        Task<bool> RegisterUserAsync(string username, string password);
    }
}