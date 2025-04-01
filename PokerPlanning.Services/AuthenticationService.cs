using BCrypt.Net;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace PokerPlanning.Services
{
    public class AuthenticationService
    {
        private readonly IUserService _userService;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public AuthenticationService(IUserService userService, AuthenticationStateProvider authenticationStateProvider)
        {
            _userService = userService;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            var user = await _userService.GetUserAsync(username);
            if (user == null)
            {
                return false;
            }

            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return false;
            }

            var claims = new List<Claim> {
            new Claim(ClaimTypes.Name, username)
        };
            var identity = new ClaimsIdentity(claims, "redisAuth");
            var principal = new ClaimsPrincipal(identity);
            var state = new AuthenticationState(principal);

            ((CustomAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(principal);
            return true;
        }

        public async Task LogoutAsync()
        {
            ((CustomAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
        }
    }
}
