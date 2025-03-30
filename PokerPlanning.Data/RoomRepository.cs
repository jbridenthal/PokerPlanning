namespace PokerPlanning.Data
{
    public class RoomRepository : BaseRepository, IRoomRepository
    {
        private const string HASH_KEY = "pokerPlanning.Rooms";
        public IEnumerable<Room> GetRooms()
        {
            var allRoom = DBContext.HashGetAll(HASH_KEY);
            var returnRoom = new List<Room>();
            foreach (var item in allRoom)
            {
                returnRoom.Add(new Room { Name = item.Name, Id = new Guid(item.Value.ToString()) });
            }
            return returnRoom;
        }

        public async Task AddRoomAsync(Room room)
        {
            if (DBContext.HashExists(HASH_KEY, room.Name))
            {
                throw new InvalidOperationException("Room name already exists");
            }
            else
            {
                await DBContext.HashSetAsync(HASH_KEY, room.Name, Guid.NewGuid().ToString());
            }
        }
        public async Task DeleteRoomAsync(Room room)
        {
            await DeleteRoomAsync(room.Name);
        }

        private async Task DeleteRoomAsync(string roomName)
        {
            await DBContext.HashDeleteAsync(HASH_KEY, roomName);
        }

        public async Task UpdateRoomAsync(Room room)
        {
            if (DBContext.HashExists(HASH_KEY, room.Name))
            {
                throw new InvalidOperationException("Team name already exists");
            }
            else
            {
                var allTeams = await DBContext.HashGetAllAsync(HASH_KEY);
                var oldTeamKey = allTeams.Where(x => x.Value == room.Id.ToString()).FirstOrDefault().Name;
                await DeleteRoomAsync(oldTeamKey.ToString());
                await DBContext.HashSetAsync(HASH_KEY, room.Name, room.Id.ToString());
            }
        }
    }
}

