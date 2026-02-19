namespace PokerPlanning.Client.Services
{
    public class HubService : IHubService, IAsyncDisposable
    {
        public delegate void UserUpdated(object sender, User user);
        public delegate void UsersUpdated(object sender, Dictionary<string, User> users);
        public delegate void RoomsUpdated(object sender, Dictionary<string, List<string>> rooms);
        public delegate void ShowVotesUpdated(object sender);
        public delegate void ClearVotesUpdated(object sender);
        public delegate void UserIDUpdated(object sender, string id);
        public delegate void UserLogOut(object sender);
        public delegate void ErrorOccurred(object sender, string errorMessage);
        public delegate void ConnectionStatusChanged(object sender, bool isConnected);

        public event Action? OnChange;
        public event UserUpdated? OnUserUpdated;
        public event UsersUpdated? OnUsersUpdated;
        public event RoomsUpdated? OnRoomsUpdated;
        public event ShowVotesUpdated? OnShowVotes;
        public event ClearVotesUpdated? OnClearVotes;
        public event UserIDUpdated? OnUserIDUpdated;
        public event UserLogOut? OnUserLogOut;
        public event ErrorOccurred? OnError;
        public event ConnectionStatusChanged? OnConnectionStatusChanged;

        private void UpdateUser() => OnUserUpdated?.Invoke(this, CurrentUser);
        private void UpdateUsers() => OnUsersUpdated?.Invoke(this, Users);
        private void UpdateRooms() => OnRoomsUpdated?.Invoke(this, Rooms);
        private void UpdateShowVotes() => OnShowVotes?.Invoke(this);
        private void UpdateClearVotes() => OnClearVotes?.Invoke(this);
        private void UpdateUserID() => OnUserIDUpdated?.Invoke(this, userId);
        private void UpdateUserLogout() => OnUserLogOut?.Invoke(this);
        private void NotifyError(string message) => OnError?.Invoke(this, message);
        private void NotifyConnectionStatus(bool isConnected) => OnConnectionStatusChanged?.Invoke(this, isConnected);

        public User CurrentUser { get; set; } = new();

        private const string LOCAL_STORAGE_NAME = "PokerPlanning.UserInfo";
        private HubConnection? hubConnection;
        private readonly NavigationManager navigationManager;
        private readonly ILocalStorageService localStorage;
        private string userName = string.Empty;
        private string userId = string.Empty;
        private Role role = Role.Observer;
        private Dictionary<string, User> Users = new Dictionary<string, User>();
        private Dictionary<string, List<string>> Rooms = new Dictionary<string, List<string>>();

        public HubService(NavigationManager navigationManager, ILocalStorageService localStorageService)
        {
            this.navigationManager = navigationManager;
            this.localStorage = localStorageService;
        }

        public async Task Connect(string setName, Role setRole)
        {
            if (string.IsNullOrWhiteSpace(setName))
            {
                NotifyError("Username cannot be empty");
                return;
            }

            userName = setName;
            role = setRole;
            await Connect();
        }

        public async Task Connect()
        {
            try
            {
                if (hubConnection != null && hubConnection.State != HubConnectionState.Disconnected)
                {
                    return; // Already connected or connecting
                }

                hubConnection = new HubConnectionBuilder()
                    .WithUrl(navigationManager.ToAbsoluteUri($"/pokerhub?username={Uri.EscapeDataString(userName)}&role={role}"))
                    .WithAutomaticReconnect(new[] { TimeSpan.Zero, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(10) })
                    .Build();

                CurrentUser = new User
                {
                    Name = userName,
                    Role = role
                };

                await localStorage.SetItemAsync(LOCAL_STORAGE_NAME, CurrentUser);

                // Register message handlers
                hubConnection.On<string>("ReceiveUserId", (connectionId) =>
                {
                    userId = connectionId;
                    CurrentUser.ID = connectionId;
                    UpdateUserID();
                    NotifyStateChanged();
                });

                hubConnection.On<Dictionary<string, User>>("ReceiveUsers", (users) =>
                {
                    Users = users ?? new Dictionary<string, User>();
                    UpdateUsers();
                    NotifyStateChanged();
                });

                hubConnection.On("ShowVotes", () =>
                {
                    UpdateShowVotes();
                    NotifyStateChanged();
                });

                hubConnection.On<Dictionary<string, List<string>>>("ReceiveRooms", (rooms) =>
                {
                    Rooms = rooms ?? new Dictionary<string, List<string>>();
                    UpdateRooms();
                    NotifyStateChanged();
                });

                hubConnection.On("ClearVotes", () =>
                {
                    UpdateClearVotes();
                    NotifyStateChanged();
                });

                hubConnection.On<string>("Error", (errorMessage) =>
                {
                    NotifyError(errorMessage);
                    NotifyStateChanged();
                });

                hubConnection.Reconnecting += async error =>
                {
                    NotifyConnectionStatus(false);
                };

                hubConnection.Reconnected += async connectionId =>
                {
                    NotifyConnectionStatus(true);
                    NotifyStateChanged();
                };

                hubConnection.Closed += async error =>
                {
                    if (error != null)
                    {
                        NotifyError($"Connection closed: {error.Message}");
                    }
                    NotifyConnectionStatus(false);
                };

                await hubConnection.StartAsync();
                NotifyConnectionStatus(true);
            }
            catch (HttpRequestException httpEx)
            {
                NotifyError($"Connection failed: {httpEx.Message}");
                NotifyConnectionStatus(false);
            }
            catch (Exception ex)
            {
                NotifyError($"An error occurred while connecting: {ex.Message}");
                NotifyConnectionStatus(false);
            }
        }

        public async Task SendVote(string vote)
        {
            try
            {
                if (!IsConnected)
                {
                    NotifyError("Not connected to server");
                    return;
                }

                if (string.IsNullOrWhiteSpace(vote))
                {
                    NotifyError("Vote cannot be empty");
                    return;
                }

                if (hubConnection != null)
                {
                    await hubConnection.SendAsync("SendVote", vote);
                    NotifyStateChanged();
                }
            }
            catch (Exception ex)
            {
                NotifyError($"Failed to send vote: {ex.Message}");
            }
        }

        public async Task ClearVotes()
        {
            try
            {
                if (!IsConnected)
                {
                    NotifyError("Not connected to server");
                    return;
                }

                if (hubConnection != null)
                {
                    await hubConnection.SendAsync("ClearVotes");
                    NotifyStateChanged();
                }
            }
            catch (Exception ex)
            {
                NotifyError($"Failed to clear votes: {ex.Message}");
            }
        }

        public async Task ShowVotes()
        {
            try
            {
                if (!IsConnected)
                {
                    NotifyError("Not connected to server");
                    return;
                }

                if (hubConnection != null)
                {
                    await hubConnection.SendAsync("ShowVotes");
                    NotifyStateChanged();
                }
            }
            catch (Exception ex)
            {
                NotifyError($"Failed to show votes: {ex.Message}");
            }
        }

        public async Task JoinRoom(string room)
        {
            try
            {
                if (!IsConnected)
                {
                    NotifyError("Not connected to server");
                    return;
                }

                if (string.IsNullOrWhiteSpace(room))
                {
                    NotifyError("Room name cannot be empty");
                    return;
                }

                if (hubConnection != null)
                {
                    await hubConnection.SendAsync("JoinRoom", room);
                    NotifyStateChanged();
                }
            }
            catch (Exception ex)
            {
                NotifyError($"Failed to join room: {ex.Message}");
            }
        }

        public async Task UpdateUser(User user)
        {
            try
            {
                if (user == null)
                {
                    NotifyError("User cannot be null");
                    return;
                }

                if (!IsConnected)
                {
                    NotifyError("Not connected to server");
                    return;
                }

                if (string.IsNullOrWhiteSpace(user.Name))
                {
                    NotifyError("User name cannot be empty");
                    return;
                }

                if (hubConnection != null)
                {
                    await hubConnection.SendAsync("UpdateUser", user);
                    CurrentUser.Name = user.Name;
                    CurrentUser.Role = user.Role;
                    await localStorage.SetItemAsync(LOCAL_STORAGE_NAME, CurrentUser);
                    NotifyStateChanged();
                }
            }
            catch (Exception ex)
            {
                NotifyError($"Failed to update user: {ex.Message}");
            }
        }

        public async Task Logout()
        {
            try
            {
                if (hubConnection != null)
                {
                    await localStorage.RemoveItemAsync(LOCAL_STORAGE_NAME);
                    UpdateUserLogout();
                    await hubConnection.SendAsync("Logout");
                    await hubConnection.StopAsync();
                }
            }
            catch (Exception ex)
            {
                NotifyError($"Failed to logout: {ex.Message}");
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (hubConnection != null)
            {
                try
                {
                    await hubConnection.DisposeAsync();
                }
                catch (Exception ex)
                {
                    NotifyError($"Error disposing hub connection: {ex.Message}");
                }
            }
            NotifyStateChanged();
        }

        // Legacy method for backward compatibility
        public async Task Dispose()
        {
            await DisposeAsync();
        }

        public bool IsConnected => hubConnection?.State == HubConnectionState.Connected;

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
