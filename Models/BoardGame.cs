namespace CafeDiceRoller.Models;

/// <summary>
/// A board game in the café's collection.
/// Field set is fixed by the assignment brief; do not remove fields.
/// </summary>
public class BoardGame
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int MinPlayers { get; set; }
    public int MaxPlayers { get; set; }
    public int PlayTimeMinutes { get; set; }
    public string Category { get; set; } = "";
    public int Rating { get; set; } // dice roll, 1-6
    public string Description { get; set; } = "";

    /// <summary>True when <paramref name="players"/> is within [MinPlayers, MaxPlayers].</summary>
    public bool SupportsPlayerCount(int players) =>
        players >= MinPlayers && players <= MaxPlayers;
}