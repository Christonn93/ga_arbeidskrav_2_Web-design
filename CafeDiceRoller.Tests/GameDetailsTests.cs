using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Bunit;
using CafeDiceRoller.Components.Pages;
using CafeDiceRoller.Models;
using CafeDiceRoller.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CafeDiceRoller.Tests;

/// <summary>
/// bUnit tests for the game details page: the "next game" link and the
/// reload-on-Id-change behaviour.
/// </summary>
public class GameDetailsTests
{
    private static readonly List<BoardGame> Catalog =
    [
        new BoardGame { Id = 1, Name = "Catan", Category = "Strategy", MinPlayers = 3, MaxPlayers = 4, PlayTimeMinutes = 75, Rating = 5, Description = "Trade." },
        new BoardGame { Id = 2, Name = "Azul", Category = "Abstract", MinPlayers = 2, MaxPlayers = 4, PlayTimeMinutes = 40, Rating = 4, Description = "Tiles." },
    ];

    private sealed class ApiHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var path = request.RequestUri!.AbsolutePath;

            if (path == "/api/games")
            {
                return Ok(JsonSerializer.Serialize(Catalog));
            }

            var id = int.Parse(path.Split('/').Last());
            var game = Catalog.FirstOrDefault(g => g.Id == id);

            return game is null
                ? Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound))
                : Ok(JsonSerializer.Serialize(game));
        }

        private static Task<HttpResponseMessage> Ok(string json) =>
            Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json"),
            });
    }

    private static TestContext NewContext()
    {
        var ctx = new TestContext();
        var client = new GameApiClient(
            new HttpClient(new ApiHandler()) { BaseAddress = new Uri("http://localhost/") });
        ctx.Services.AddSingleton(client);
        ctx.Services.AddSingleton(new GameNightState());
        return ctx;
    }

    [Fact]
    public void Links_to_the_next_game_in_the_catalog()
    {
        using var ctx = NewContext();

        var cut = ctx.RenderComponent<GameDetails>(p => p.Add(x => x.Id, 1));

        Assert.Contains("Neste spill", cut.Markup);
        var link = cut.Find("a");
        Assert.Equal("/games/2", link.GetAttribute("href"));
    }

    [Fact]
    public void Next_game_link_wraps_to_the_first_game_at_the_end()
    {
        using var ctx = NewContext();

        var cut = ctx.RenderComponent<GameDetails>(p => p.Add(x => x.Id, 2));

        var link = cut.Find("a");
        Assert.Equal("/games/1", link.GetAttribute("href"));
    }

    [Fact]
    public void Reload_on_id_change_shows_the_new_game()
    {
        using var ctx = NewContext();

        var cut = ctx.RenderComponent<GameDetails>(p => p.Add(x => x.Id, 1));
        Assert.Contains("Catan", cut.Markup);

        cut.SetParametersAndRender(p => p.Add(x => x.Id, 2));

        Assert.Contains("Azul", cut.Markup);
    }
}
