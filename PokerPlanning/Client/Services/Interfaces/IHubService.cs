namespace PokerPlanning.Client.Services.Interfaces
{
    public interface IHubService
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

        Task ClearVotes();
        Task Connect();
        Task Connect(string setName, Enums.Role setRole);
        Task Dispose();
        Task JoinRoom(string room);
        Task Logout();
        Task SendVote(string vote);
        Task ShowVotes();
        Task UpdateUser(User user);
    }
}