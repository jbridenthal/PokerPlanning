using Microsoft.AspNetCore.SignalR;
using PokerPlanning.Shared;
using static PokerPlanning.Shared.Enums;

namespace PokerPlanning.Server.Hubs
{
    public class PokerHub : Hub
    {
        private static Dictionary<string, User> Users = new Dictionary<string, User>();
        private static Dictionary<string,List<string>> Rooms = new Dictionary<string, List<string>>();
        const string ROOM_LOBBY = "Lobby";
        public override async Task OnConnectedAsync()
        {
            var request = Context.GetHttpContext()?.Request;

            string? username = request != null ? request.Query["username"] : string.Empty;
            string? role  = request != null ? request.Query["role"] : Role.Observer.ToString();
            string? room = request != null ? request.Query["room"] : ROOM_LOBBY;

            Users.Add(Context.ConnectionId, new User { Name = username, Role = (Role)Enum.Parse(typeof(Role), role ?? Role.Observer.ToString()) });
            await JoinRoom(room ?? ROOM_LOBBY);
            await SendUsers();
            await SendRooms();
            await SendUserId();
            await base.OnConnectedAsync();
        }

        public Task ThrowException()
        {
            throw new HubException("This error will be sent to the client!");
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var user = Users.FirstOrDefault(u => u.Key == Context.ConnectionId);
            Users.Remove(Context.ConnectionId);
            await LeaveRoom(user.Value.Room ?? "");
            await SendUsers();
            await SendRooms();
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendUsers()
        {
            await Clients.All.SendAsync("RecieveUsers", Users);
        }

        public async Task SendRooms()
        {
            await Clients.All.SendAsync("RecieveRooms", Rooms);
        }

        public async Task SendVote(string vote)
        {
            Users.Where(x => x.Key == Context.ConnectionId).FirstOrDefault().Value.Vote = vote;
            await SendUsers();
        }

        public async Task ShowVotes()
        {

            if (Users.ContainsKey(Context.ConnectionId) && Users[Context.ConnectionId].Room != null)
            {
                await Clients.Group(Users[Context.ConnectionId].Room ?? "").SendAsync("ShowVotes");
            }
        }

        public async Task ClearVotes()
        {
            var room =  Users[Context.ConnectionId].Room;
            foreach (var key in Users.Keys)
            {
                if (Users[key].Room == room) { 
                     Users[key].Vote = null;
                }
            }
            await SendUsers();
        }


        public async Task SendUserId()
        {
            await Clients.Caller.SendAsync("RecieveUserId", Context.ConnectionId);
        }

        public async Task JoinRoom(string room) 
        {
            if(Users[Context.ConnectionId] != null && Users[Context.ConnectionId].Room != null)
            {
                await LeaveRoom(Users[Context.ConnectionId].Room ?? ""); 
            }
            Users[Context.ConnectionId].Room = room;

            if(Rooms.ContainsKey(room))
            {
                Rooms[room].Add(Context.ConnectionId);
            } 
            else
            {
                Rooms.Add(room, new List<string> { Context.ConnectionId});
            }
            await Groups.AddToGroupAsync(Context.ConnectionId, room);
            await SendRooms();
            await SendUsers();
        }

        public async Task LeaveRoom(string room)
        {
            if (Rooms[room] != null)
            {
                if (Rooms[room].Count == 1) { 
                Rooms.Remove(room); 
                } 
                else {
                    Rooms[room].Remove(Context.ConnectionId);
                 }
            }
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, room);
            await SendRooms();
            await SendUsers();
        }
    }
}
