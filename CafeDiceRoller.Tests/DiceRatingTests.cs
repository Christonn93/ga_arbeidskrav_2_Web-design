using Bunit;
using CafeDiceRoller.Components.Ui.Rating;
using Xunit;

namespace CafeDiceRoller.Tests;

/// <summary>
/// bUnit tests for the clickable and read-only dice rating component.
/// </summary>
public class DiceRatingTests
{
    [Fact]
    public void Default_value_is_one()
    {
        using var ctx = new TestContext();

        var cut = ctx.RenderComponent<DiceRating>();

        Assert.Contains("1/6", cut.Markup);
    }

    [Fact]
    public void Click_cycles_to_next_value()
    {
        using var ctx = new TestContext();
        int? received = null;
        var cut = ctx.RenderComponent<DiceRating>(p => p
            .Add(x => x.Value, 1)
            .Add(x => x.ValueChanged, (int v) => { received = v; }));

        cut.Find("button").Click();

        Assert.Equal(2, received);
    }

    [Fact]
    public void Click_at_six_wraps_to_one()
    {
        using var ctx = new TestContext();
        int received = 0;
        var cut = ctx.RenderComponent<DiceRating>(p => p
            .Add(x => x.Value, 6)
            .Add(x => x.ValueChanged, (int v) => { received = v; }));

        cut.Find("button").Click();

        Assert.Equal(1, received);
    }

    [Fact]
    public void ReadOnly_renders_no_click_target()
    {
        using var ctx = new TestContext();
        var cut = ctx.RenderComponent<DiceRating>(p => p
            .Add(x => x.Value, 5)
            .Add(x => x.ReadOnly, true));

        Assert.Empty(cut.FindAll("button"));
        Assert.Contains("5/6", cut.Markup);
    }

    [Theory]
    [InlineData(0, "1/6")]
    [InlineData(7, "6/6")]
    public void Out_of_range_value_is_clamped(int value, string expected)
    {
        using var ctx = new TestContext();
        var cut = ctx.RenderComponent<DiceRating>(p => p.Add(x => x.Value, value));

        Assert.Contains(expected, cut.Markup);
    }
}
