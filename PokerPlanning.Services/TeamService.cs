namespace PokerPlanning.Services
{
    public class TeamService : ITeamService
    {
        private ITeamRepository _repo;
        public TeamService(ITeamRepository teamRepository) { _repo = teamRepository; }
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
