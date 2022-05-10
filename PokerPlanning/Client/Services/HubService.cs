
using static PokerPlanning.Shared.Enums;

namespace PokerPlanning.Client.Services
{

    public class HubService
    {
        public delegate void UserUpdated(object sender, User user);
        public delegate void UsersUpdated(object sender, Dictionary<string, User> users);
        public delegate void RoomsUpdated(object sender, Dictionary<string, List<string>> rooms);
        public delegate void ShowVotesUpdated(object sender);
        public delegate void ClearVotesUpdated(object sender);
        public delegate void UserIDUpdated(object sender, string id);
        public delegate void UserLogOut(object sender);

        public event Action? OnChange;
        public event UserUpdated? OnUserUpdated;
        public event UsersUpdated? OnUsersUpdated;
        public event RoomsUpdated? OnRoomsUpdated;
        public event ShowVotesUpdated? OnShowVotes;
        public event ClearVotesUpdated? OnClearVotes;
        public event UserIDUpdated? OnUserIDUpdated;
        public event UserLogOut? OnUserLogOut;

        private void UpdateUser() => OnUserUpdated?.Invoke(this, CurrentUser);
        private void UpdateUsers() => OnUsersUpdated?.Invoke(this, Users);
        private void UpdateRooms() => OnRoomsUpdated?.Invoke(this, Rooms);
        private void UpdateShowVotes() => OnShowVotes?.Invoke(this);
        private void UpdateClearVotes() => OnClearVotes?.Invoke(this);
        private void UpdateUserID() => OnUserIDUpdated?.Invoke(this, userId);
        private void UpdateUserLogout() => OnUserLogOut.Invoke(this);

        public User CurrentUser { get; set; } = new();

        const string LOCAL_STORAGE_NAME = "PokerPlanning.UserInfo";
        private HubConnection? hubConnection;
        private NavigationManager NavigationManager;
        private ILocalStorageService localStorage;
        private string userName { get; set; } = string.Empty;
        private string userId = string.Empty;
        private Role role = Role.Observer;
        private Dictionary<string, User> Users = new Dictionary<string, User>();
        private static Dictionary<string, List<string>> Rooms = new Dictionary<string, List<string>>();

        public HubService(NavigationManager navigationManager, ILocalStorageService localStorageService)
        {
            this.NavigationManager = navigationManager;
            this.localStorage = localStorageService;
        }

        public async Task Connect(string setName, Role setRole)
        {
            userName = setName;
            role = setRole;
            await Connect();
        }

        public async Task Connect()
        {
            hubConnection = new HubConnectionBuilder()
            .WithUrl(NavigationManager.ToAbsoluteUri($"/pokerhub?username={userName}&role={role}"))
            .WithAutomaticReconnect()
            .Build();

            CurrentUser = new User
            {
                Name = userName,
                Role = role
            };

            await localStorage.SetItemAsync(LOCAL_STORAGE_NAME, CurrentUser);

            hubConnection.On<string>("RecieveUserId", (connectionId) =>
            {
                userId = connectionId;
                CurrentUser.ID = connectionId;
                UpdateUserID();
                NotifyStateChanged();
            });

            hubConnection.On<Dictionary<string, User>>("RecieveUsers", (users) =>
            {
                Users = users;
                UpdateUsers();
                NotifyStateChanged();
            });


            hubConnection.On("ShowVotes", () =>
            {
                UpdateShowVotes();
                NotifyStateChanged();
            });


            hubConnection.On<Dictionary<string, List<string>>>("RecieveRooms", (rooms) =>
            {
                Rooms = rooms;
                UpdateRooms();
                NotifyStateChanged();
            });

            hubConnection.On("ClearVotes", () =>
            {
                UpdateClearVotes();
                NotifyStateChanged();
            });

            try
            {
                await hubConnection.StartAsync();
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }

        public async Task SendVote(string vote)
        {
            if (hubConnection != null)
            {
                await hubConnection.SendAsync("SendVote", vote);
                NotifyStateChanged();
            }
        }

        public async Task ClearVotes()
        {
            if (hubConnection != null)
            {
                await hubConnection.SendAsync("ClearVotes");
                NotifyStateChanged();
            }
        }

        public async Task ShowVotes()
        {
            if (hubConnection != null)
            {
                await hubConnection.SendAsync("ShowVotes");
                NotifyStateChanged();
            }
        }

        public async Task JoinRoom(string room)
        {
            if (hubConnection != null)
            {
                await hubConnection.SendAsync("JoinRoom", room);
                NotifyStateChanged();
            }
        }

        public async Task UpdateUser(User user)
        {
            if (hubConnection != null)
            {
                await hubConnection.SendAsync("UpdateUser", user);
                CurrentUser.Name = user.Name;
                CurrentUser.Role = user.Role;
                await localStorage.SetItemAsync(LOCAL_STORAGE_NAME, CurrentUser);
                NotifyStateChanged();
            }
        }

        public async Task Logout()
        {
            if (hubConnection != null)
            {
                await localStorage.RemoveItemAsync(LOCAL_STORAGE_NAME);
                UpdateUserLogout();
                await hubConnection.SendAsync("Logout");
            }
        }

        public async Task Dispose()
        {
            if (hubConnection != null)
            {
                await hubConnection.DisposeAsync();
                NotifyStateChanged();
            }
        }

        public bool IsConnected => hubConnection?.State == HubConnectionState.Connected;

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
