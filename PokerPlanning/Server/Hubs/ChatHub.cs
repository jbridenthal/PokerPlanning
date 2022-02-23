using Microsoft.AspNetCore.SignalR;
using PokerPlanning.Shared;
using static PokerPlanning.Shared.Enums;

namespace PokerPlanning.Server.Hubs
{
    public class ChatHub : Hub
    {
        private static Dictionary<string, User> Users = new Dictionary<string, User>();

        public override async Task OnConnectedAsync()
        {
            var request = Context.GetHttpContext()?.Request;

            string? username = request != null ? request.Query["username"] : string.Empty;
            string? role  = request != null ? request.Query["role"] : Role.Observer.ToString();
            Users.Add(Context.ConnectionId, new User { Name = username, Role = (Role)Enum.Parse(typeof(Role), role ?? Role.Observer.ToString()) });

            await SendMessage(string.Empty, $"{username}({role}) {Context.ConnectionId} Connected!");
            await SendUsers();
            await base.OnConnectedAsync();

        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            string username = Users.FirstOrDefault(u => u.Key == Context.ConnectionId).Value.Name ?? String.Empty;
            Users.Remove(Context.ConnectionId);
            await SendUsers();
            await SendMessage(string.Empty, $"{username} Disconnected!");
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("RecieveMessage", user, message);
        }

        public async Task SendUsers()
        {
            await Clients.All.SendAsync("RecieveUsers", Users);
        }

        public async Task SendVote(string vote)
        {
            Users.Where(x => x.Key == Context.ConnectionId).FirstOrDefault().Value.Vote = vote;
            await SendMessage(Users.FirstOrDefault(u => u.Key == Context.ConnectionId).Value.Name ?? String.Empty, $"voted a {vote}");
            await SendUsers();
        }

        public async Task ClearVotes()
        {
            foreach(var key in Users.Keys)
            {
                Users[key].Vote = null;
            }
            await SendUsers();
            await SendMessage(Users.FirstOrDefault(u => u.Key == Context.ConnectionId).Value.Name ?? String.Empty, $"cleared votes");
        }
    }
}
