using System.ComponentModel.DataAnnotations;

namespace CafeDiceRoller.Models;

public sealed class GameFormModel : IValidatableObject
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Navn er påkrevd")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Kategori er påkrevd")]
    public string Category { get; set; } = string.Empty;

    [Range(1, 99, ErrorMessage = "MinPlayers må være minst 1")]
    public int MinPlayers { get; set; } = 2;

    [Range(1, 99, ErrorMessage = "MaxPlayers må være minst 1")]
    public int MaxPlayers { get; set; } = 4;

    [Range(1, 10000, ErrorMessage = "Spilletid må være over 0")]
    public int PlayTimeMinutes { get; set; } = 60;

    [Range(1, 6, ErrorMessage = "Terningkast må være fra 1 til 6")]
    public int Rating { get; set; } = 1;

    public string Description { get; set; } = string.Empty;

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