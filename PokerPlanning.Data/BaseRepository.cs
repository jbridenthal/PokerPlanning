namespace PokerPlanning.Data
{
    public class BaseRepository
    {
        public IDatabase DBContext;
        public BaseRepository()
        {
            ConfigurationOptions conf = new ConfigurationOptions
            {
                EndPoints = { "localhost:6379" },
                Password = "your_secure_password" //todo move to config, probably pull in through docker-compose file 
            };
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(conf);
            DBContext = redis.GetDatabase();
        }
    }
}