namespace PokerPlanning.Data.Interfaces
{
    public interface IRedisAuthRepository
    {
        Task AddUserAsync(User user);
        Task<User> GetUserAsync(string username);
    }
}