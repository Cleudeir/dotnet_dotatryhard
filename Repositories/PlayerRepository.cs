using dotatryhard.Data;
using dotatryhard.Interfaces;
using dotatryhard.Models;

namespace dotatryhard.Repositories
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public PlayerRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task InsertPlayersAsync(List<int> players) { }
    }
}
