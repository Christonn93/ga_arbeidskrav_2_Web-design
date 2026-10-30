using CafeDiceRoller.Models;
using CafeDiceRoller.Services;
using Xunit;

namespace CafeDiceRoller.Tests;

/// <summary>
/// Tests for the scoped game-night state used by the shelf, game page,
/// game-night page, and header badge.
/// </summary>
public class GameNightStateTests
{
    private static BoardGame Game(int id, int minutes = 60) => new()
    {
        Id = id,
        Name = $"Game {id}",
        MinPlayers = 2,
        MaxPlayers = 4,
        PlayTimeMinutes = minutes,
    };

    [Fact]
    public void Add_game_increases_count()
    {
        var state = new GameNightState();

        Assert.True(state.Add(Game(1)));

        Assert.Equal(1, state.Count);
    }

    [Fact]
    public void Add_duplicate_is_ignored()
    {
        var state = new GameNightState();
        var game = Game(1);

        Assert.True(state.Add(game));
        Assert.False(state.Add(game));

        Assert.Equal(1, state.Count);
        Assert.Single(state.Games);
    }

    [Fact]
    public void Remove_existing_game_decreases_count()
    {
        var state = new GameNightState();
        state.Add(Game(1));

        Assert.True(state.Remove(1));

        Assert.Equal(0, state.Count);
    }

    [Fact]
    public void Remove_missing_game_returns_false()
    {
        var state = new GameNightState();

        Assert.False(state.Remove(99));
        Assert.Equal(0, state.Count);
    }

    [Fact]
    public void TotalPlayTime_sums_all_selected_games()
    {
        var state = new GameNightState();

        state.Add(Game(1, minutes: 45));
        state.Add(Game(2, minutes: 60));
        state.Add(Game(3, minutes: 90));

        Assert.Equal(195, state.TotalPlayTimeMinutes);
    }

    [Fact]
    public void TotalPlayTime_is_zero_when_empty()
    {
        var state = new GameNightState();

        Assert.Equal(0, state.TotalPlayTimeMinutes);
    }

    [Fact]
    public void Contains_tracks_added_and_removed_games()
    {
        var state = new GameNightState();
        state.Add(Game(7));

        Assert.True(state.Contains(7));
        Assert.False(state.Contains(8));

        state.Remove(7);

        Assert.False(state.Contains(7));
    }

    [Fact]
    public void OnChange_fires_on_mutation_but_not_on_duplicate_add()
    {
        var state = new GameNightState();
        var notifications = 0;
        state.OnChange += () => notifications++;

        state.Add(Game(1));   // fires
        state.Add(Game(1));   // duplicate: no change, no event
        state.Remove(1);      // fires

        Assert.Equal(2, notifications);
    }
}
