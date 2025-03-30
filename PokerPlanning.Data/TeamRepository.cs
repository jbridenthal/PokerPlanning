using PokerPlanning.Models;
using PokerPlanning.Shared;
using StackExchange.Redis;

namespace PokerPlanning.Data
{
    public class TeamRepository : BaseRepository
    {
        private const string HASH_KEY = "pokerPlanning.Teams";

        public IEnumerable<Team> GetTeams()
        {
            var allTeams = DBContext.HashGetAll(HASH_KEY);
            var returnTeams = new List<Team>();
            foreach (var item in allTeams)
            {
                returnTeams.Add(new Team { Name = item.Name, Id = new Guid(item.Value.ToString()) });
            }
            return returnTeams;
        }

        public async Task AddTeamAsync(Team team)
        {
            if (DBContext.HashExists(HASH_KEY, team.Name))
            {
                throw new InvalidOperationException("Team name already exists");
            } else
            {
                await DBContext.HashSetAsync(HASH_KEY, team.Name, Guid.NewGuid().ToString());
            }
        }

        public async Task DeleteTeamAsync(Team team)
        {
            await DeleteTeamAsync(team.Name);
        }
        private  async Task DeleteTeamAsync(string teamName)
        {
            await DBContext.HashDeleteAsync(HASH_KEY, teamName);
        }
        public async Task UpdateTeamAsync(Team team)
        {
            if (DBContext.HashExists(HASH_KEY, team.Name))
            {
                throw new InvalidOperationException("Team name already exists");
            }
            else
            {
                var allTeams = await DBContext.HashGetAllAsync(HASH_KEY);
                var oldTeamKey = allTeams.Where(x => x.Value == team.Id.ToString()).FirstOrDefault().Name;
                await DeleteTeamAsync(oldTeamKey.ToString());
                await DBContext.HashSetAsync(HASH_KEY, team.Name, team.Id.ToString());
            }
        }
    }
}
