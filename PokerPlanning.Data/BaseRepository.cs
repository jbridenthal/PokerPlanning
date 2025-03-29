using JsonFlatFileDataStore;

namespace PokerPlanning.Data
{
    public class BaseRepository
    {
        public DataStore TeamDatastore;
        public BaseRepository()
        {
             TeamDatastore = new DataStore("teamData.json");
        }
    }
}