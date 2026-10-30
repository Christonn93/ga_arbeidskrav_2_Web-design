using Bunit;
using CafeDiceRoller.Components.Shared;
using CafeDiceRoller.Models;
using Xunit;

namespace CafeDiceRoller.Tests;

/// <summary>
/// bUnit tests for the admin form's client-side validation (empty name and
/// invalid player range render validation messages without a request).
/// </summary>
public class GameFormValidationTests
{
    private static GameFormModel Valid() => new()
    {
        Name = "Catan",
        Category = "Strategy",
        MinPlayers = 3,
        MaxPlayers = 4,
        PlayTimeMinutes = 75,
        Rating = 5,
    };

    [Fact]
    public void Empty_name_shows_required_error()
    {
        using var ctx = new TestContext();
        var model = Valid();
        model.Name = "";

        var cut = ctx.RenderComponent<GameForm>(p => p.Add(x => x.Model, model));

        cut.Find("form").Submit();

        Assert.Contains("Navn er påkrevd", cut.Markup);
    }

    [Fact]
    public void MinPlayers_below_range_shows_range_error()
    {
        using var ctx = new TestContext();
        var model = Valid();
        model.MinPlayers = 0;

        var cut = ctx.RenderComponent<GameForm>(p => p.Add(x => x.Model, model));

        cut.Find("form").Submit();

        Assert.Contains("MinPlayers må være minst 1", cut.Markup);
    }

    [Fact]
    public void MaxPlayers_below_min_shows_cross_field_error()
    {
        using var ctx = new TestContext();
        var model = Valid();
        model.MinPlayers = 4;
        model.MaxPlayers = 2;

        var cut = ctx.RenderComponent<GameForm>(p => p.Add(x => x.Model, model));

        cut.Find("form").Submit();

        Assert.Contains("MaxPlayers kan ikke være lavere enn MinPlayers", cut.Markup);
    }
}
