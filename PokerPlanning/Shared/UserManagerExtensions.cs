
using Microsoft.AspNetCore.Identity;
using PokerPlanning.Models;
using System.Threading.Tasks;

namespace PokerPlanning.Shared
{
    public static class UserManagerExtensions
        {
            public static async Task<IdentityResult> SetUserNamesAsync(this UserManager<ApplicationUser> userManager, ApplicationUser user, string firstName, string lastName)
            {
                user.FirstName = firstName;
                user.LastName = lastName;
                return await userManager.UpdateAsync(user);
            }

            public static async Task<string?> GetFirstNameAsync(this UserManager<ApplicationUser> userManager, ApplicationUser user)
            {
                return await Task.FromResult(user.FirstName);
            }

            public static async Task<string?> GetLastNameAsync(this UserManager<ApplicationUser> userManager, ApplicationUser user)
            {
                return await Task.FromResult(user.LastName);
            }
        }
    }

