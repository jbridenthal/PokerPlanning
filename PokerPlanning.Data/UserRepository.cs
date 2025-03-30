using Blazored.LocalStorage;
using PokerPlanning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerPlanning.Data
{
    public class UserRepository
    {
        ILocalStorageService _localStorageService;
        const string LOCAL_STORAGE_NAME = "PokerPlanning.UserInfo";
        public UserRepository(ILocalStorageService localStorageService) { _localStorageService = localStorageService; }

        public async Task GetUserAsync()
        {
            var test = await _localStorageService.GetItemAsync<User>(LOCAL_STORAGE_NAME);
        }
        
    }
}
