using CafeDiceRoller.Models;
using Microsoft.EntityFrameworkCore;

namespace CafeDiceRoller.Data;

/// <summary>
/// EF Core context for the café's game collection.
/// One <see cref="DbSet{TEntity}"/> per aggregate root — currently just games.
/// </summary>
public class GameDbContext : DbContext
{
    // Accepts options configured via AddDbContext in Program.cs.
    public GameDbContext(DbContextOptions<GameDbContext> options)
        : base(options)
    {
    }

    // Games table exposed as a queryable set.
    public DbSet<BoardGame> Games => Set<BoardGame>();

    // Configures column constraints to match UI validation.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<BoardGame>(entity =>
        {
            entity.Property(g => g.Name)
                .IsRequired()
                .HasMaxLength(120);

            entity.Property(g => g.Category)
                .IsRequired()
                .HasMaxLength(60);

            entity.Property(g => g.Description)
                .HasMaxLength(1000);

            // Matches the assignment's "dice roll, 1-6" rule.
            entity.ToTable(t => t.HasCheckConstraint(
                "CK_BoardGame_Rating",
                "\"Rating\" BETWEEN 1 AND 6"));

            // Matches "MinPlayers >= 1" and "MaxPlayers >= MinPlayers".
            entity.ToTable(t => t.HasCheckConstraint(
                "CK_BoardGame_PlayerRange",
                "\"MinPlayers\" >= 1 AND \"MaxPlayers\" >= \"MinPlayers\""));

            // Matches "PlayTimeMinutes > 0".
            entity.ToTable(t => t.HasCheckConstraint(
                "CK_BoardGame_PlayTime",
                "\"PlayTimeMinutes\" > 0"));
        });
    }
}