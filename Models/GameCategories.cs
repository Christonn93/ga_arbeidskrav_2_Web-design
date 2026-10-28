namespace CafeDiceRoller.Models;

public static class GameCategories
{
    // Fixed list of categories used across the app.
    public static readonly IReadOnlyList<string> All =
    [
        "Strategy",
        "Family",
        "Party",
        "Abstract",
        "Cooperative",
        "Tile-laying",
        "Card drafting"
    ];
}