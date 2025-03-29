using PokerPlanning.Data;
using PokerPlanning.Models;

namespace PokerPlanning.Services
{
    public class TeamService
    {
        private TeamRepository _repo;
        public TeamService(TeamRepository teamRepository) { _repo = teamRepository; }
        public IEnumerable<Team> GetTeams(params int[] Ids)
        {
            return _repo.GetTeams(Ids);
        }
        public IEnumerable<Team> GetTeams()
        {
            return _repo.GetTeams();
        }

        public async Task<bool> AddTeamAsync(Team team)
        {
            return await _repo.AddTeamAsync(team);
        }

        public async Task<bool> DeleteTeamAsync(int Id)
        {
            return await _repo.DeleteTeamAsync(Id);
        }

        public async Task<bool> UpdateTeamAsync(Team team)
        {
            return await _repo.UpdateTeamAsync(team);
        }
    }
}
