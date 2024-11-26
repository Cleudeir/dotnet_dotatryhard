using dotatryhard.Models;
using Microsoft.EntityFrameworkCore;

namespace dotatryhard.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : DbContext(options)
    {
        // Define DbSet properties for your models
        public required DbSet<Match> matches { get; set; }
        public required DbSet<Player> players { get; set; }
        public required DbSet<PlayersMatches> players_matches { get; set; }
    }
}
