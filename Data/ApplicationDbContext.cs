using Microsoft.EntityFrameworkCore;
using dotatryhard.Models;


namespace dotatryhard.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Define DbSet properties for your models
        public DbSet<Product> Products { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<PlayersMatches> PlayersMatches { get; set; }
    }
}
