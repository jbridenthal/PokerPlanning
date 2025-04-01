using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;

namespace PokerPlanning.Services
{
    public class UserService : IUserService
    {
        private readonly IRedisAuthRepository _redisRepository;

        public UserService(IRedisAuthRepository redisRepository)
        {
            _redisRepository = redisRepository;
        }

        public async Task<User> GetUserAsync(string username)
        {
            return await _redisRepository.GetUserAsync(username);
        }

        public async Task<bool> RegisterUserAsync(string username, string password)
        {
            if (await GetUserAsync(username) != null)
            {
                return false; // User already exists
            }

            string salt = BCrypt.Net.BCrypt.GenerateSalt();
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password, salt);

            var user = new User
            {
                Name = username,
                PasswordHash = passwordHash,
                Salt = salt
            };

            await _redisRepository.AddUserAsync(user);
            return true;
        }
    }
}
