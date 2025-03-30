namespace PokerPlanning.Services
{
    public class RoomService : IRoomService
    {
        private IRoomRepository _repo;
        public RoomService(IRoomRepository roomRepository) { _repo = roomRepository; }

        public IEnumerable<Room> GetRooms()
        {
            return _repo.GetRooms();
        }

        public async Task AddRoomAsync(Room room)
        {
            await _repo.AddRoomAsync(room);
        }

        public async Task DeleteRoomAsync(Room room)
        {
            await _repo.DeleteRoomAsync(room);
        }

        public async Task UpdateRoomAsync(Room room)
        {
            await _repo.UpdateRoomAsync(room);
        }
    }
}
