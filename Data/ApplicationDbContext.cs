using dotatryhard.Models; // Import the namespace containing the entity models (Match, Player, PlayersMatches)
using Microsoft.EntityFrameworkCore; // Import Entity Framework Core for database interactions

namespace dotatryhard.Data
{

    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public required DbSet<Match> Matches { get; set; }
        public required DbSet<Player> Players { get; set; }
        public required DbSet<PlayersMatches> PlayersMatches { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            base.OnModelCreating(modelBuilder);

            // Configure primary key for Match
            modelBuilder.Entity<Match>()
                .HasKey(m => m.match_id);

            // Configure primary key for Player
            modelBuilder.Entity<Player>()
                .HasKey(p => p.account_id);

            // Configure composite primary key for PlayersMatches
            modelBuilder.Entity<PlayersMatches>()
                .HasKey(pm => new { pm.account_id, pm.match_id });

            // Configure relationships
            modelBuilder.Entity<PlayersMatches>()
                .HasOne(pm => pm.player)
                .WithMany(p => p.PlayersMatches)
                .HasForeignKey(pm => pm.account_id);

            modelBuilder.Entity<PlayersMatches>()
                .HasOne(pm => pm.match)
                .WithMany(m => m.PlayersMatches)
                .HasForeignKey(pm => pm.match_id);
        }
    }
}
