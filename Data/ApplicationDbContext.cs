using dotatryhard.Models;
using Microsoft.EntityFrameworkCore;

namespace dotatryhard.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : DbContext(options)
    {
        public required DbSet<Match> Matches { get; set; }
        public required DbSet<Player> Players { get; set; }
        public required DbSet<PlayersMatches> PlayersMatches { get; set; }
        public required DbSet<PlayersMatchesAverages> PlayersMatchesAverages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure primary key for Match
            modelBuilder.Entity<Match>().HasKey(m => m.match_id);

            // Configure primary key for Player
            modelBuilder.Entity<Player>().HasKey(p => p.account_id);

            // Configure composite primary key for PlayersMatches
            modelBuilder.Entity<PlayersMatches>().HasKey(pm => new { pm.match_id, pm.account_id }); // Set composite primary key here

            // Configure primary key for PlayersMatchesAverages
            modelBuilder.Entity<PlayersMatchesAverages>().HasKey(pm => new { pm.account_id });

            // PlayersMatches relationships
            modelBuilder
                .Entity<PlayersMatches>()
                .HasOne(pm => pm.player)
                .WithMany(p => p.PlayersMatches)
                .HasForeignKey(pm => pm.account_id);

            modelBuilder
                .Entity<PlayersMatches>()
                .HasOne(pm => pm.match)
                .WithMany(m => m.PlayersMatches)
                .HasForeignKey(pm => pm.match_id);

            // PlayersMatchesAverages relationships
            modelBuilder
                .Entity<PlayersMatchesAverages>()
                .HasOne(pm => pm.player)
                .WithMany(p => p.PlayersMatchesAverages)
                .HasForeignKey(pm => pm.account_id);
        }
    }
}
