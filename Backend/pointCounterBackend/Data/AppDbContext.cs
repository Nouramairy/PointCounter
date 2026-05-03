using Microsoft.EntityFrameworkCore;
using pointCounterBackend.Entities;

namespace pointCounterBackend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Player> Players { get; set; } = null!;
        public DbSet<Team> Teams { get; set; } = null!;
        public DbSet<Game> Games { get; set; } = null!;
        public DbSet<Scoreboard> Scoreboards { get; set; } = null!;
        public DbSet<TeamPlayer> TeamPlayers { get; set; } = null!;
        public DbSet<GameTeam> GameTeams { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigurePlayer(modelBuilder);
            ConfigureTeam(modelBuilder);
            ConfigureGame(modelBuilder);
            ConfigureScoreboard(modelBuilder);
            ConfigureTeamPlayer(modelBuilder);
            ConfigureGameTeam(modelBuilder);
        }

        // serperation of concern.
        private static void ConfigurePlayer(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Player>(entity =>
            { 
                entity.ToTable("Players");

                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name).IsRequired().HasMaxLength(50);
                entity.Property(p => p.Age).IsRequired();
                entity.Property(p => p.Address).IsRequired().HasMaxLength(100);
                entity.Property(p => p.Phone).IsRequired();
                entity.Property(p => p.CreatedAt).IsRequired();
                entity.Property(p => p.UpdatedAt).IsRequired();

                // 1. what heppens if one of the field lets say , address is missing?
                // 2. why name field's max length is 50
            });
        }

        private static void ConfigureTeam(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Team>(entity =>
            {
                entity.ToTable("Teams");
                entity.HasKey(entity => entity.Id);
                entity.Property(t => t.Name).IsRequired().HasMaxLength(50);
                entity.Property(t => t.MaximumPlayersAllowed).IsRequired();
                entity.Property(t => t.CreatedAt).IsRequired();
                entity.Property(t => t.UpdatedAt).IsRequired();


            });
            
        }

        private static void ConfigureGame(ModelBuilder modelBuilder)
        {
                modelBuilder.Entity<Game>(entity =>
                {
                    entity.ToTable("Games");
                    entity.HasKey(g => g.Id);
                    entity.Property(g => g.Name).IsRequired().HasMaxLength(50);
                    entity.Property(g => g.Duration) .IsRequired();
                    entity.Property(g => g.CreatedAt).IsRequired();
                    entity.Property(g => g.UpdatedAt).IsRequired();
                });
        }

        private static void ConfigureScoreboard(ModelBuilder modelBuilder)
        {
                modelBuilder.Entity<Scoreboard>(entity =>
                {
                    entity.ToTable("Scoreboards");
                    entity.HasKey(s => s.Id);
                    entity.Property(s => s.Score).IsRequired();
                    entity.Property(s => s.CreatedAt).IsRequired();
                    entity.Property(s => s.UpdatedAt).IsRequired();

                    entity.HasOne(s => s.Game)
                        .WithMany(g => g.Scoreboards)
                        .HasForeignKey(s => s.GameId)
                        .OnDelete(DeleteBehavior.Cascade);
                    // what the behavior cascade means?

                    entity.HasOne(s => s.Team)
                        .WithMany()
                        .HasForeignKey(s => s.TeamId)
                        .OnDelete(DeleteBehavior.Restrict);
                    // what the behavior Restrict means?
                    // team -> scores , here team can exist without scores
                    // but score cant exist without team
                    // if we delete the game/team, the score will be deleted

                    entity.HasIndex(s => new { s.GameId, s.TeamId })
                        .IsUnique();
                });
        }

        private static void ConfigureTeamPlayer(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TeamPlayer>(entity =>
            {
                entity.ToTable("TeamPlayers");

                entity.HasKey(tp => new { tp.TeamId, tp.PlayerId });

                entity.HasOne(tp => tp.Team)
                    .WithMany(t => t.TeamPlayers)
                    .HasForeignKey(tp => tp.TeamId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(tp => tp.Player)
                    .WithMany(p => p.TeamPlayers)
                    .HasForeignKey(tp => tp.PlayerId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        private static void ConfigureGameTeam(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GameTeam>(entity =>
            {
                entity.ToTable("GameTeams");

                entity.HasKey(gt => new { gt.GameId, gt.TeamId });

                entity.HasOne(gt => gt.Game)
                    .WithMany(g => g.GameTeams)
                    .HasForeignKey(gt => gt.GameId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(gt => gt.Team)
                    .WithMany(t => t.GameTeams)
                    .HasForeignKey(gt => gt.TeamId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
