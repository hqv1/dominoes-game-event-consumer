using Microsoft.EntityFrameworkCore;

namespace Hqv.Dominoes.GameEventConsumer.App.Data
{
    public class DominoesContext : DbContext
    {
        public DominoesContext(DbContextOptions<DominoesContext> options)
            :base(options)
        {
        }
        
        public DbSet<Game> Games { get; set; } = null!;
        public DbSet<GameEvent> GameEvents { get; set; } = null!;
        public DbSet<GamePlayer> GamePlayers { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Game>()
                .HasMany(x => x.GamePlayers)
                .WithOne(x => x.Game)
                .HasForeignKey(x => x.GameId);
        
            modelBuilder.Entity<Game>()
                .HasMany(x => x.GameEvents)
                .WithOne(x => x.Game)
                .HasForeignKey(x => x.GameId);
        }
    }
}