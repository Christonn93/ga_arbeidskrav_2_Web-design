using Bunit;
using CafeDiceRoller.Components.Ui.Card;
using CafeDiceRoller.Models;
using Xunit;

namespace CafeDiceRoller.Tests;

/// <summary>
/// bUnit tests for the shelf card: rendering and the add-to-game-night event.
/// </summary>
public class GameCardTests
{
    private static BoardGame Game(int id = 1) => new()
    {
        Id = id,
        Name = "Catan",
        Category = "Strategy",
        Description = "Trade and build.",
        MinPlayers = 3,
        MaxPlayers = 4,
        PlayTimeMinutes = 75,
        Rating = 5,
    };

    [Fact]
    public void Renders_game_name_and_link()
    {
        using var ctx = new TestContext();

        var cut = ctx.RenderComponent<GameCard>(p => p.Add(x => x.Game, Game()));

        Assert.Contains("Catan", cut.Markup);
        Assert.Contains("/games/1", cut.Markup);
    }

    [Fact]
    public void Clicking_add_raises_OnAddToGameNight()
    {
        using var ctx = new TestContext();
        BoardGame? added = null;
        var cut = ctx.RenderComponent<GameCard>(p => p
            .Add(x => x.Game, Game())
            .Add(x => x.OnAddToGameNight, g => { added = g; }));

        cut.Find("button").Click();

        Assert.NotNull(added);
        Assert.Equal("Catan", added!.Name);
    }

    [Fact]
    public void IsInGameNight_disables_the_add_button()
    {
        using var ctx = new TestContext();
        var cut = ctx.RenderComponent<GameCard>(p => p
            .Add(x => x.Game, Game())
            .Add(x => x.IsInGameNight, true));

        var button = cut.Find("button");

        Assert.Contains("Lagt til", button.TextContent);
        Assert.True(button.HasAttribute("disabled"));
    }
}
