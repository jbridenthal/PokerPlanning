namespace PokerPlanning.Services.Interfaces
{
    public interface ITeamService
    {
        Task AddTeamAsync(Team team);
        Task DeleteTeamAsync(Team team);
        IEnumerable<Team> GetTeams();
        Task UpdateTeamAsync(Team team);
    }
}