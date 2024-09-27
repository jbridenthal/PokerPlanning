using Microsoft.AspNetCore.Identity;
using PokerPlanning.Models;
using System.Security.Claims;

namespace PokerPlanning.Services
{
    public class UserService
    {
        private IHttpContextAccessor _context;
        private UserManager<ApplicationUser> _userManager = default!;
        private ApplicationUser user = default!;
        public UserService(IHttpContextAccessor context, UserManager<ApplicationUser> userManage)
        {
            _context = context;
            _userManager = userManage;
        }
        
        public async Task<ApplicationUser?> GetUser()
        {
            if (_context.HttpContext?.User.Identity?.IsAuthenticated == true)
            {
                user = await _userManager.GetUserAsync(_context.HttpContext?.User) ?? default!;
            }
            return user;
        }

    }
}
