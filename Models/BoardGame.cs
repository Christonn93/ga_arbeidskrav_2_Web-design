namespace CafeDiceRoller.Models;

/// <summary>
/// A board game in the café's collection.
/// Field set is fixed by the assignment brief; do not remove fields.
/// </summary>
public class BoardGame
{
    // Unique identifier for the game.
    public int Id { get; set; }
    // Display name of the game.
    public string Name { get; set; } = "";
    // Minimum supported player count.
    public int MinPlayers { get; set; }
    // Maximum supported player count.
    public int MaxPlayers { get; set; }
    // Approximate play time in minutes.
    public int PlayTimeMinutes { get; set; }
    // Category label (e.g. Strategy, Party).
    public string Category { get; set; } = "";
    // Dice roll rating, 1-6.
    public int Rating { get; set; }
    // Short description of the game.
    public string Description { get; set; } = "";

    // True when <paramref name="players"/> is within [MinPlayers, MaxPlayers].
    public bool SupportsPlayerCount(int players) => players >= MinPlayers && players <= MaxPlayers;
}