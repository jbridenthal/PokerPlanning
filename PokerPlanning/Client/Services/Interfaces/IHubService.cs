namespace PokerPlanning.Client.Services.Interfaces
{
    public interface IHubService : IAsyncDisposable
    {
        User CurrentUser { get; set; }
        bool IsConnected { get; }

        event Action? OnChange;
        event HubService.ClearVotesUpdated? OnClearVotes;
        event HubService.RoomsUpdated? OnRoomsUpdated;
        event HubService.ShowVotesUpdated? OnShowVotes;
        event HubService.UserIDUpdated? OnUserIDUpdated;
        event HubService.UserLogOut? OnUserLogOut;
        event HubService.UsersUpdated? OnUsersUpdated;
        event HubService.UserUpdated? OnUserUpdated;
        event HubService.ErrorOccurred? OnError;
        event HubService.ConnectionStatusChanged? OnConnectionStatusChanged;

        Task ClearVotes();
        Task Connect();
        Task Connect(string setName, Role setRole);
        Task Dispose();
        Task JoinRoom(string room);
        Task Logout();
        Task SendVote(string vote);
        Task ShowVotes();
        Task UpdateUser(User user);
    }
}