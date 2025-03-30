namespace PokerPlanning.Services.Interfaces
{
    public interface IRoomService
    {
        Task AddRoomAsync(Room room);
        Task DeleteRoomAsync(Room room);
        IEnumerable<Room> GetRooms();
        Task UpdateRoomAsync(Room room);
    }
}