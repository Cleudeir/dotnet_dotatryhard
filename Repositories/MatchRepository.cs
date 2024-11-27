using dotatryhard.Data;
using dotatryhard.Interfaces;
using dotatryhard.Models;

namespace dotatryhard.Repositories
{
    public class MatchRepository : IMatchRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public MatchRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task InsertMatchesAsync(List<long> matches) { }
    }
}
