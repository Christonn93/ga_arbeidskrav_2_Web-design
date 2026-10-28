using System.ComponentModel.DataAnnotations;

namespace CafeDiceRoller.Models;

public sealed class GameFormModel : IValidatableObject
{
    // Set when editing an existing game; 0 for new.
    public int Id { get; set; }

    // Display name of the game.
    [Required(ErrorMessage = "Navn er påkrevd")]
    public string Name { get; set; } = string.Empty;

    // Category label for the game.
    [Required(ErrorMessage = "Kategori er påkrevd")]
    public string Category { get; set; } = string.Empty;

    // Minimum supported players.
    [Range(1, 99, ErrorMessage = "MinPlayers må være minst 1")]
    public int MinPlayers { get; set; } = 2;

    // Maximum supported players.
    [Range(1, 99, ErrorMessage = "MaxPlayers må være minst 1")]
    public int MaxPlayers { get; set; } = 4;

    // Approximate play time in minutes.
    [Range(1, 10000, ErrorMessage = "Spilletid må være over 0")]
    public int PlayTimeMinutes { get; set; } = 60;

    // Dice roll rating, 1-6.
    [Range(1, 6, ErrorMessage = "Terningkast må være fra 1 til 6")]
    public int Rating { get; set; } = 1;

    // Short description of the game.
    public string Description { get; set; } = string.Empty;

    // Cross-field validation for min/max players.
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (MaxPlayers < MinPlayers)
        {
            yield return new ValidationResult(
                "MaxPlayers kan ikke være lavere enn MinPlayers",
                [nameof(MaxPlayers)]);
        }
    }
}