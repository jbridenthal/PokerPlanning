using System.Text.Json;

namespace PokerPlanning.Data
{

    public class RedisAuthRepository : BaseRepository, IRedisAuthRepository
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _db;

        public RedisAuthRepository(IConnectionMultiplexer redis)
        {
            _redis = redis;
            _db = _redis.GetDatabase();
        }

        public async Task<User> GetUserAsync(string username)
        {
            var value = await _db.StringGetAsync($"user:{username}");
            if (value.IsNullOrEmpty)
            {
                return null;
            }
            return JsonSerializer.Deserialize<User>(value);
        }

        public async Task AddUserAsync(User user)
        {
            await _db.StringSetAsync($"user:{user.Name}", JsonSerializer.Serialize(user));
        }
    }
}



//class RedisAuth
//{
//    private static readonly ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
//    private static readonly IDatabase db = redis.GetDatabase();

//    public static async Task RegisterUser(string username, string password)
//    {
//        string passwordHash = ComputeSha256Hash(password);

//        await db.HashSetAsync($"user:{username}", new HashEntry[]
//        {
//            new HashEntry("password", passwordHash),
//            new HashEntry("email", $"{username}@example.com"),
//            new HashEntry("role", "user")
//        });

//        Console.WriteLine("User registered successfully.");
//    }

//    private static string ComputeSha256Hash(string rawData)
//    {
//        using SHA256 sha256Hash = SHA256.Create();
//        byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
//        StringBuilder builder = new StringBuilder();
//        foreach (byte t in bytes)
//        {
//            builder.Append(t.ToString("x2"));
//        }
//        return builder.ToString();
//    }

//    static async Task Main()
//    {
//        await RegisterUser("alice", "SecurePassword123");
//    }
//}
