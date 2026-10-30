using CafeDiceRoller.Services;
using Xunit;

namespace CafeDiceRoller.Tests;

/// <summary>
/// Tests for the shelf filtering logic (name search + player-count filter).
/// </summary>
public class GameDataSearchTests
{
    private readonly GameData _data = new();

    [Theory]
    [InlineData("catan")]
    [InlineData("CATAN")]
    [InlineData("CaTaN")]
    public void Search_is_case_insensitive(string query)
    {
        var results = _data.Search(query, null);

        Assert.Contains(results, g => g.Name == "Catan");
    }

    [Fact]
    public void Search_with_no_match_returns_empty()
    {
        Assert.Empty(_data.Search("zzzzz", null));
    }

    [Fact]
    public void Search_by_player_count_uses_inclusive_range()
    {
        // Catan supports 3-4 players in the seeded catalog.
        Assert.Contains(_data.Search(null, 3), g => g.Name == "Catan");
        Assert.Contains(_data.Search(null, 4), g => g.Name == "Catan");
        Assert.DoesNotContain(_data.Search(null, 2), g => g.Name == "Catan");
        Assert.DoesNotContain(_data.Search(null, 5), g => g.Name == "Catan");
    }

    [Fact]
    public void Combined_search_and_filter_intersect()
    {
        // Games whose name contains "a" AND support 3 players.
        var names = _data.Search("a", 3)
            .Select(g => g.Name)
            .OrderBy(n => n)
            .ToList();

        Assert.Equal(new[] { "Azul", "Carcassonne", "Catan", "Pandemic" }, names);
    }
}
