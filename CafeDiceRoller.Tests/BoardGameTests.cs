using CafeDiceRoller.Models;
using Xunit;

namespace CafeDiceRoller.Tests;

/// <summary>
/// Tests for the player-count rule on <see cref="BoardGame"/>, which backs both
/// the page filter and <see cref="Services.GameData"/>.
/// </summary>
public class BoardGameTests
{
    private static BoardGame TwoToFour() => new()
    {
        Id = 1,
        Name = "Catan",
        MinPlayers = 2,
        MaxPlayers = 4,
    };

    [Theory]
    // Boundary and off-by-one cases mapped in the manual test log (T1.3).
    [InlineData(1, false)]
    [InlineData(2, true)]
    [InlineData(3, true)]
    [InlineData(4, true)]
    [InlineData(5, false)]
    public void SupportsPlayerCount_respects_inclusive_boundaries(int players, bool expected)
    {
        Assert.Equal(expected, TwoToFour().SupportsPlayerCount(players));
    }

    [Fact]
    public void SupportsPlayerCount_handles_single_player_bands()
    {
        var game = new BoardGame { MinPlayers = 2, MaxPlayers = 2 };

        Assert.False(game.SupportsPlayerCount(1));
        Assert.True(game.SupportsPlayerCount(2));
        Assert.False(game.SupportsPlayerCount(3));
    }
}
