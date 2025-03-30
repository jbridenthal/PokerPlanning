using PokerPlanning.Data;
using PokerPlanning.Models;

namespace PokerPlanning.Services
{
    public class RoomService
    {
        private RoomRepository _repo;
        public RoomService(RoomRepository roomRepository) { _repo = roomRepository; }
        public IEnumerable<Room> GetRooms(params int[] Ids)
        {
            return _repo.GetRooms(Ids);
        }
        public IEnumerable<Room> GetRooms()
        {
            return _repo.GetRooms();
        }

        public async Task<bool> AddRoomAsync(Room Room)
        {
            return await _repo.AddRoomAsync(Room);
        }

        public async Task<bool> DeleteRoomAsync(int Id)
        {
            return await _repo.DeleteRoomAsync(Id);
        }

        public async Task<bool> UpdateRoomAsync(Room Room)
        {
            return await _repo.UpdateRoomAsync(Room);
        }
    }
}
