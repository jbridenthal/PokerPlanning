using PokerPlanning.Models;
using PokerPlanning.Shared;

namespace PokerPlanning.Data
{
    public class RoomRepository : BaseRepository
    {
        public IEnumerable<Room> GetRooms(params int[] Ids)
        {
            var collection = RoomDatastore.GetCollection<Room>();
            return collection.AsQueryable().Where(x => x.Id.In(Ids)) ?? new List<Room>();
        }
        public IEnumerable<Room> GetRooms()
        {
            return RoomDatastore.GetCollection<Room>().AsQueryable();
        }

        public async Task<bool> AddRoomAsync(Room Room)
        {
            var collection = RoomDatastore.GetCollection<Room>();
            if (collection.AsQueryable().Any(x => x.Name == Room.Name))
            {
                throw new InvalidOperationException("Room name already exists");
            }
            return await collection.InsertOneAsync(Room);
        }

        public async Task<bool> DeleteRoomAsync(int Id)
        {
            var collection = RoomDatastore.GetCollection<Room>();
            return await collection.DeleteOneAsync(Id);
        }

        public async Task<bool> UpdateRoomAsync(Room Room)
        {
            var collection = RoomDatastore.GetCollection<Room>();
            if (collection.AsQueryable().Any(x => x.Name == Room.Name))
            {
                throw new InvalidOperationException("Room name already exists");
            }
            return await collection.UpdateOneAsync(x => x.Id == Room.Id, Room);
        }
    }
}

