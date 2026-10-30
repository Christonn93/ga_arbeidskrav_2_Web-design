using CafeDiceRoller.Models;
using Xunit;

namespace CafeDiceRoller.Tests;

/// <summary>
/// Tests for the server-side validation used by the REST API endpoints.
/// BoardGameValidator lives in the global namespace, mirroring the DB
/// check constraints in GameDbContext.
/// </summary>
public class BoardGameValidatorTests
{
    private static BoardGame Valid() => new()
    {
        Id = 1,
        Name = "Catan",
        Category = "Strategy",
        MinPlayers = 3,
        MaxPlayers = 4,
        PlayTimeMinutes = 75,
        Rating = 5,
    };

    [Fact]
    public void Valid_game_has_no_errors()
    {
        Assert.Empty(BoardGameValidator.Validate(Valid()));
    }

    [Fact]
    public void Empty_name_is_rejected()
    {
        var game = Valid();
        game.Name = "";

        Assert.Contains(BoardGameValidator.Validate(game), e => e.Key == nameof(BoardGame.Name));
    }

    [Fact]
    public void Whitespace_name_is_rejected()
    {
        var game = Valid();
        game.Name = "   ";

        Assert.Contains(BoardGameValidator.Validate(game), e => e.Key == nameof(BoardGame.Name));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-3)]
    public void MinPlayers_below_one_is_rejected(int minPlayers)
    {
        var game = Valid();
        game.MinPlayers = minPlayers;

        Assert.Contains(BoardGameValidator.Validate(game), e => e.Key == nameof(BoardGame.MinPlayers));
    }

    [Fact]
    public void MaxPlayers_below_min_is_rejected()
    {
        var game = Valid();
        game.MinPlayers = 4;
        game.MaxPlayers = 2;

        Assert.Contains(BoardGameValidator.Validate(game), e => e.Key == nameof(BoardGame.MaxPlayers));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Non_positive_playtime_is_rejected(int minutes)
    {
        var game = Valid();
        game.PlayTimeMinutes = minutes;

        Assert.Contains(BoardGameValidator.Validate(game), e => e.Key == nameof(BoardGame.PlayTimeMinutes));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(7)]
    public void Rating_outside_dice_range_is_rejected(int rating)
    {
        var game = Valid();
        game.Rating = rating;

        Assert.Contains(BoardGameValidator.Validate(game), e => e.Key == nameof(BoardGame.Rating));
    }

    [Fact]
    public void Empty_category_is_rejected()
    {
        var game = Valid();
        game.Category = "";

        Assert.Contains(BoardGameValidator.Validate(game), e => e.Key == nameof(BoardGame.Category));
    }

    [Fact]
    public void Boundary_values_are_accepted()
    {
        var game = Valid();
        game.MinPlayers = 1;
        game.MaxPlayers = 1;
        game.PlayTimeMinutes = 1;
        game.Rating = 1;

        Assert.Empty(BoardGameValidator.Validate(game));

        game.Rating = 6;
        Assert.Empty(BoardGameValidator.Validate(game));
    }
}
