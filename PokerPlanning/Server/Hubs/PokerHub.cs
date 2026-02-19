using Microsoft.AspNetCore.SignalR;
using PokerPlanning.Shared;
using System.Collections.Concurrent;

namespace PokerPlanning.Server.Hubs
{
    public class PokerHub : Hub
    {
        private static readonly ConcurrentDictionary<string, User> Users = new ConcurrentDictionary<string, User>();
        private static readonly ConcurrentDictionary<string, List<string>> Rooms = new ConcurrentDictionary<string, List<string>>();
        private static readonly object RoomsLock = new object();
        const string ROOM_LOBBY = "Lobby";

        private readonly ILogger<PokerHub> _logger;

        public PokerHub(ILogger<PokerHub> logger)
        {
            _logger = logger;
        }

        // Helper method to get all users in a specific room
        private List<User> GetUsersInRoom(string room)
        {
            return Users.Values.Where(u => u.Room == room).ToList();
        }

        // Helper method to notify a room of state changes
        private async Task NotifyRoomAsync(string room)
        {
            await Clients.Group(room).SendAsync("ReceiveUsers", Users);
        }
        public override async Task OnConnectedAsync()
        {
            try
            {
                var request = Context.GetHttpContext()?.Request;

                string? username = request?.Query["username"].ToString();
                string? roleString = request?.Query["role"].ToString();
                string? room = request?.Query["room"].ToString();

                // Validate and parse role
                if (!Enum.TryParse<Role>(roleString ?? Role.Observer.ToString(), out var role))
                {
                    role = Role.Observer;
                }

                username = string.IsNullOrWhiteSpace(username) ? "Anonymous" : username;
                room = string.IsNullOrWhiteSpace(room) ? ROOM_LOBBY : room;

                if (!Users.TryAdd(Context.ConnectionId, new User { Name = username, Role = role }))
                {
                    _logger.LogWarning("User {ConnectionId} already exists in Users dictionary", Context.ConnectionId);
                }

                _logger.LogInformation("User {ConnectionId} ({Username}) connected as {Role}", Context.ConnectionId, username, role);

                await SendUserId();
                await JoinRoom(room);
                await SendUsers();
                await SendRooms();
                await base.OnConnectedAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during OnConnectedAsync for connection {ConnectionId}", Context.ConnectionId);
                throw;
            }
        }

        public Task ThrowException()
        {
            throw new HubException("This error will be sent to the client!");
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            try
            {
                if (Users.TryRemove(Context.ConnectionId, out var user))
                {
                    _logger.LogInformation("User {ConnectionId} ({Username}) disconnected", Context.ConnectionId, user.Name);

                    if (!string.IsNullOrWhiteSpace(user.Room))
                    {
                        await LeaveRoom(user.Room);
                    }
                }
                else
                {
                    _logger.LogWarning("User {ConnectionId} not found during disconnection", Context.ConnectionId);
                }

                await SendUsers();
                await SendRooms();
                await base.OnDisconnectedAsync(exception);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during OnDisconnectedAsync for connection {ConnectionId}", Context.ConnectionId);
                // Don't throw - disconnection errors should be logged but not rethrown
            }
        }

        public async Task SendUsers()
        {
            try
            {
                await Clients.All.SendAsync("ReceiveUsers", Users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending users to clients");
            }
        }

        public async Task SendRooms()
        {
            try
            {
                await Clients.All.SendAsync("ReceiveRooms", Rooms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending rooms to clients");
            }
        }

        public async Task SendVote(string vote)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(vote) || !Users.TryGetValue(Context.ConnectionId, out var user))
                {
                    return;
                }

                user.Vote = vote;
                _logger.LogDebug("User {ConnectionId} voted: {Vote}", Context.ConnectionId, vote);
                await SendUsers();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending vote for user {ConnectionId}", Context.ConnectionId);
                await Clients.Caller.SendAsync("Error", "Failed to submit vote");
            }
        }

        public async Task ShowVotes()
        {
            try
            {
                if (Users.TryGetValue(Context.ConnectionId, out var currentUser) && !string.IsNullOrWhiteSpace(currentUser.Room))
                {
                    _logger.LogDebug("User {ConnectionId} showing votes in room {Room}", Context.ConnectionId, currentUser.Room);
                    await Clients.Group(currentUser.Room).SendAsync("ShowVotes");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error showing votes for user {ConnectionId}", Context.ConnectionId);
                await Clients.Caller.SendAsync("Error", "Failed to show votes");
            }
        }

        public async Task ClearVotes()
        {
            try
            {
                if (!Users.TryGetValue(Context.ConnectionId, out var currentUser) || string.IsNullOrWhiteSpace(currentUser.Room))
                {
                    return;
                }

                var room = currentUser.Room;
                var usersInRoom = GetUsersInRoom(room);

                foreach (var user in usersInRoom)
                {
                    user.Vote = null;
                }

                _logger.LogDebug("Votes cleared in room {Room} by user {ConnectionId}", room, Context.ConnectionId);
                await Clients.Group(room).SendAsync("ClearVotes");
                await SendUsers();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing votes for user {ConnectionId}", Context.ConnectionId);
                await Clients.Caller.SendAsync("Error", "Failed to clear votes");
            }
        }


        public async Task SendUserId()
        {
            try
            {
                await Clients.Caller.SendAsync("ReceiveUserId", Context.ConnectionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending user ID to {ConnectionId}", Context.ConnectionId);
            }
        }

        public async Task JoinRoom(string room) 
        {
            try
            {
                if (string.IsNullOrWhiteSpace(room) || !Users.TryGetValue(Context.ConnectionId, out var user))
                {
                    return;
                }

                // Leave previous room if different
                if (!string.IsNullOrWhiteSpace(user.Room) && user.Room != room)
                {
                    await LeaveRoom(user.Room);
                }

                user.Room = room;

                lock (RoomsLock)
                {
                    if (Rooms.TryGetValue(room, out var connectionIds))
                    {
                        if (!connectionIds.Contains(Context.ConnectionId))
                        {
                            connectionIds.Add(Context.ConnectionId);
                        }
                    }
                    else
                    {
                        Rooms.TryAdd(room, new List<string> { Context.ConnectionId });
                    }
                }

                await Groups.AddToGroupAsync(Context.ConnectionId, room);
                _logger.LogInformation("User {ConnectionId} ({Username}) joined room {Room}", Context.ConnectionId, user.Name, room);

                await SendRooms();
                await SendUsers();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error joining room {Room} for user {ConnectionId}", room, Context.ConnectionId);
                await Clients.Caller.SendAsync("Error", "Failed to join room");
            }
        }

        public async Task ChangeName(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name) || !Users.TryGetValue(Context.ConnectionId, out var user))
                {
                    return;
                }

                var oldName = user.Name;
                user.Name = name;
                _logger.LogInformation("User {ConnectionId} changed name from {OldName} to {NewName}", Context.ConnectionId, oldName, name);

                await SendUsers();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing name for user {ConnectionId}", Context.ConnectionId);
                await Clients.Caller.SendAsync("Error", "Failed to change name");
            }
        }
        public async Task LeaveRoom(string room)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(room))
                {
                    return;
                }

                lock (RoomsLock)
                {
                    if (Rooms.TryGetValue(room, out var connectionIds))
                    {
                        connectionIds.Remove(Context.ConnectionId);
                        if (connectionIds.Count == 0)
                        {
                            Rooms.TryRemove(room, out _);
                        }
                    }
                }

                await Groups.RemoveFromGroupAsync(Context.ConnectionId, room);
                _logger.LogInformation("User {ConnectionId} left room {Room}", Context.ConnectionId, room);

                await SendRooms();
                await SendUsers();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error leaving room {Room} for user {ConnectionId}", room, Context.ConnectionId);
            }
        }

        public async Task UpdateUser(User user)
        {
            try
            {
                if (user == null || !Users.TryGetValue(Context.ConnectionId, out var currentUser))
                {
                    return;
                }

                bool updated = false;

                if (!string.IsNullOrWhiteSpace(user.Name) && currentUser.Name != user.Name)
                {
                    currentUser.Name = user.Name;
                    updated = true;
                }

                if (currentUser.Role != user.Role)
                {
                    currentUser.Role = user.Role;
                    updated = true;
                }

                if (updated)
                {
                    _logger.LogInformation("User {ConnectionId} updated - Name: {Name}, Role: {Role}", 
                        Context.ConnectionId, currentUser.Name, currentUser.Role);
                    await SendUsers();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user {ConnectionId}", Context.ConnectionId);
                await Clients.Caller.SendAsync("Error", "Failed to update user");
            }
        }

        public async Task Logout()
        {
            try
            {
                if (Users.TryRemove(Context.ConnectionId, out var user))
                {
                    _logger.LogInformation("User {ConnectionId} ({Username}) logged out", Context.ConnectionId, user.Name);

                    if (!string.IsNullOrWhiteSpace(user.Room))
                    {
                        await LeaveRoom(user.Room);
                    }
                }
                else
                {
                    _logger.LogWarning("User {ConnectionId} not found during logout", Context.ConnectionId);
                }

                await SendUsers();
                await SendRooms();
                Context.Abort();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout for connection {ConnectionId}", Context.ConnectionId);
                try
                {
                    await Clients.Caller.SendAsync("Error", "Failed to logout");
                }
                catch (Exception notifyEx)
                {
                    _logger.LogError(notifyEx, "Failed to notify client about logout error");
                }
            }
        }
    }
}
