using PokerPlanning.Data;
using PokerPlanning.Models;

namespace PokerPlanning.Services
{
    public class TeamService
    {
        private TeamRepository _repo;
        public TeamService(TeamRepository teamRepository) { _repo = teamRepository; }
        public IEnumerable<Team> GetTeams()
        {
            return _repo.GetTeams();
        }

        public async Task AddTeamAsync(Team team)
        {
             await _repo.AddTeamAsync(team);
        }

        public async Task DeleteTeamAsync(Team team)
        {
             await _repo.DeleteTeamAsync(team);
        }

        public async Task UpdateTeamAsync(Team team)
        {
             await _repo.UpdateTeamAsync(team);
        }
    }
}
