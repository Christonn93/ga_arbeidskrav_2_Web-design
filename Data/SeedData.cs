using CafeDiceRoller.Models;

namespace CafeDiceRoller.Data;

/// <summary>
/// Populates the database with the café's starting collection the first time
/// the app runs. Idempotent: does nothing if games already exist.
/// </summary>
public static class SeedData
{
    public static void Initialize(GameDbContext context)
    {
        if (context.Games.Any())
        {
            return;
        }

        var games = new BoardGame[]
        {
            new()
            {
                Name = "Catan",
                MinPlayers = 3, MaxPlayers = 4,
                PlayTimeMinutes = 75, Category = "Strategy",
                Rating = 5,
                Description = "Trade, build, and settle the island of Catan."
            },
            new()
            {
                Name = "Ticket to Ride",
                MinPlayers = 2, MaxPlayers = 5,
                PlayTimeMinutes = 60, Category = "Family",
                Rating = 5,
                Description = "Collect train cards and claim routes across the map."
            },
            new()
            {
                Name = "Codenames",
                MinPlayers = 4, MaxPlayers = 8,
                PlayTimeMinutes = 30, Category = "Party",
                Rating = 4,
                Description = "Give one-word clues so your team finds the right agents."
            },
            new()
            {
                Name = "Azul",
                MinPlayers = 2, MaxPlayers = 4,
                PlayTimeMinutes = 45, Category = "Abstract",
                Rating = 6,
                Description = "Draft tiles and build a mosaic on your player board."
            },
            new()
            {
                Name = "Wingspan",
                MinPlayers = 1, MaxPlayers = 5,
                PlayTimeMinutes = 90, Category = "Strategy",
                Rating = 5,
                Description = "Attract birds to your wildlife preserve."
            },
            new()
            {
                Name = "Dixit",
                MinPlayers = 3, MaxPlayers = 6,
                PlayTimeMinutes = 30, Category = "Party",
                Rating = 4,
                Description = "Tell a story with a card, and guess which card is whose."
            },
            new()
            {
                Name = "7 Wonders",
                MinPlayers = 3, MaxPlayers = 7,
                PlayTimeMinutes = 45, Category = "Strategy",
                Rating = 5,
                Description = "Draft cards to build a civilization across three ages."
            },
            new()
            {
                Name = "Carcassonne",
                MinPlayers = 2, MaxPlayers = 5,
                PlayTimeMinutes = 45, Category = "Family",
                Rating = 4,
                Description = "Place tiles to build cities, roads, and fields."
            },
            new()
            {
                Name = "The Crew",
                MinPlayers = 2, MaxPlayers = 5,
                PlayTimeMinutes = 20, Category = "Co-op",
                Rating = 3,
                Description = "Cooperate on trick-taking missions in space."
            },
            new()
            {
                Name = "Terraforming Mars",
                MinPlayers = 1, MaxPlayers = 5,
                PlayTimeMinutes = 120, Category = "Strategy",
                Rating = 6,
                Description = "Corporations compete to make Mars habitable."
            },
        };

        context.Games.AddRange(games);
        context.SaveChanges();
    }
}