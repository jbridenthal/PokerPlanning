using PokerPlanning.Models;
using PokerPlanning.Shared;

namespace PokerPlanning.Data
{
    public class TeamRepository : BaseRepository
    {
        public IEnumerable<Team> GetTeams(params int[] Ids)
        {
            var collection = TeamDatastore.GetCollection<Team>();
            return collection.AsQueryable().Where(x => x.Id.In(Ids)) ?? new List<Team>();
        }
        public IEnumerable<Team> GetTeams()
        {
            return TeamDatastore.GetCollection<Team>().AsQueryable();
        }

        public async Task<bool> AddTeamAsync(Team team)
        {
            var collection = TeamDatastore.GetCollection<Team>();
            if(collection.AsQueryable().Any(x=>x.Name == team.Name))
            {
                throw new InvalidOperationException("Team name already exists");
            }
            return await collection.InsertOneAsync(team);
        }

        public async Task<bool> DeleteTeamAsync(int Id)
        {
            var collection = TeamDatastore.GetCollection<Team>();
            return await collection.DeleteOneAsync(Id);
        }

        public async Task<bool> UpdateTeamAsync(Team team)
        {
            var collection = TeamDatastore.GetCollection<Team>();
            if (collection.AsQueryable().Any(x => x.Name == team.Name))
            {
                throw new InvalidOperationException("Team name already exists");
            }
            return await collection.UpdateOneAsync(x=>x.Id == team.Id,team);
        }
    }
}
