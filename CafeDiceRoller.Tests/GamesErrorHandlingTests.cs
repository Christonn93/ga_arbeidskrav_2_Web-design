using System.Net.Http;
using Bunit;
using CafeDiceRoller.Components.Pages;
using CafeDiceRoller.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CafeDiceRoller.Tests;

/// <summary>
/// bUnit tests for the shelf page's error handling when the API is unavailable.
/// </summary>
public class GamesErrorHandlingTests
{
    private sealed class ThrowingHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
            => throw new HttpRequestException("Simulated API outage");
    }

    [Fact]
    public void Api_failure_shows_error_card_without_crashing()
    {
        using var ctx = new TestContext();

        var client = new HttpClient(new ThrowingHandler())
        {
            BaseAddress = new Uri("http://localhost/"),
        };
        ctx.Services.AddSingleton(new GameApiClient(client));
        ctx.Services.AddSingleton(new GameNightState());

        var cut = ctx.RenderComponent<Games>();

        Assert.Contains("Could not load games", cut.Markup);
        Assert.Contains("role=\"alert\"", cut.Markup);
    }
}
