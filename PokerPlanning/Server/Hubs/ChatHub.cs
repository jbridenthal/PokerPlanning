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
            string username = Context.GetHttpContext().Request.Query["username"];
            string role = Context.GetHttpContext().Request.Query["role"];
            Users.Add(Context.ConnectionId, new User { Name = username, Role = (Role)Enum.Parse(typeof(Role), role) });

            await SendMessage(string.Empty, $"{username}({role}) {Context.ConnectionId} Connected!");
            await SendUsers();
            await base.OnConnectedAsync();

        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            string username = Users.FirstOrDefault(u => u.Key == Context.ConnectionId).Value.Name;
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
            await SendMessage(Users.FirstOrDefault(u => u.Key == Context.ConnectionId).Value.Name, $"voted a {vote}");
            await SendUsers();
        }

        public async Task ClearVotes()
        {
            foreach(var key in Users.Keys)
            {
                Users[key].Vote = null;
            }
            await SendUsers();
            await SendMessage(Users.FirstOrDefault(u => u.Key == Context.ConnectionId).Value.Name, $"cleared votes");
        }
    }
}
