// Validation/BoardGameValidator.cs

using CafeDiceRoller.Models;

public static class BoardGameValidator
{
    // Returns a dictionary of field -> error messages, empty if valid.
    public static Dictionary<string, string[]> Validate(BoardGame game)
    {
        var errors = new Dictionary<string, string[]>();

        // Name must be non-empty.
        if (string.IsNullOrWhiteSpace(game.Name))
            errors[nameof(game.Name)] = new[] { "Name is required." };

        // Minimum players must be at least 1.
        if (game.MinPlayers < 1)
            errors[nameof(game.MinPlayers)] = new[] { "MinPlayers must be at least 1." };

        // Maximum players cannot be below the minimum.
        if (game.MaxPlayers < game.MinPlayers)
            errors[nameof(game.MaxPlayers)] = new[] { "MaxPlayers cannot be lower than MinPlayers." };

        // Play time must be positive.
        if (game.PlayTimeMinutes <= 0)
            errors[nameof(game.PlayTimeMinutes)] = new[] { "PlayTimeMinutes must be greater than 0." };

        // Rating must fall within the 1-6 dice range.
        if (game.Rating is < 1 or > 6)
            errors[nameof(game.Rating)] = new[] { "Rating must be between 1 and 6." };

        // Category must be non-empty.
        if (string.IsNullOrWhiteSpace(game.Category))
            errors[nameof(game.Category)] = new[] { "Category is required." };

        return errors;
    }
}