namespace PokerPlanning.Data.Interfaces
{
    public interface IRoomRepository
    {
        Task AddRoomAsync(Room room);
        Task DeleteRoomAsync(Room room);
        IEnumerable<Room> GetRooms();
        Task UpdateRoomAsync(Room room);
    }
}