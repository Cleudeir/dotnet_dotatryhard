using dotatryhard.Models;
using Microsoft.EntityFrameworkCore;

namespace dotatryhard.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Match> Matches { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<PlayersMatches> PlayersMatches { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Match>(entity =>
            {
                entity.HasKey(m => m.match_id); // Explicitly set the primary key
            });

            modelBuilder.Entity<Player>(entity =>
            {
                entity.HasKey(p => p.account_id); // Ensure primary key for Player
            });

            modelBuilder.Entity<PlayersMatches>(entity =>
            {
                entity.HasKey(pm => new { pm.account_id, pm.match_id }); // Composite key
            });
        }
    }
}
