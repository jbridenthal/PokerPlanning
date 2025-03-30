using PokerPlanning.Models;

namespace PokerPlanning.Data.Interfaces
{
    public interface ITeamRepository
    {
        Task AddTeamAsync(Team team);
        Task DeleteTeamAsync(Team team);
        IEnumerable<Team> GetTeams();
        Task UpdateTeamAsync(Team team);
    }
}