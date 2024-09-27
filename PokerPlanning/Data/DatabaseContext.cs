using Microsoft.EntityFrameworkCore;
using PokerPlanning.Models;

namespace PokerPlanning.Data
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext() { }
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)  {  }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=pokerplanning.db");
        }

        public DbSet<RoomModel> Rooms { get; set; }

    }
}
