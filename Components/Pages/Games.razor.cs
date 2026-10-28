using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CafeDiceRoller.Models;
using CafeDiceRoller.Services;
using Microsoft.AspNetCore.Components;

namespace CafeDiceRoller.Components.Pages;

public partial class Games : IDisposable
{
    [Inject]
    private GameApiClient GameClient { get; set; } = default!;

    [Inject]
    private GameNightState Night { get; set; } = default!;

    private readonly List<BoardGame> _games = [];

    private string SearchText { get; set; } = "";

    private string PlayerCountInput { get; set; } = "";

    private int? HighlightedGameId { get; set; }

    private string? LastAddedMessage { get; set; }

    private bool _loading = true;

    private string? _loadError;

    private IReadOnlyList<BoardGame> FilteredGames =>
        _games.Where(Matches).ToList();

    protected override async Task OnInitializedAsync()
    {
        Night.OnChange += StateHasChanged;
        await Load();
    }

    public void Dispose() => Night.OnChange -= StateHasChanged;

    private async Task Load()
    {
        _loading = true;
        _loadError = null;

        try
        {
            var games = await GameClient.GetAllAsync();

            _games.Clear();
            _games.AddRange(games);
        }
        catch (HttpRequestException)
        {
            _loadError = "Could not load games.";
        }
        catch (TaskCanceledException)
        {
            _loadError = "The games request timed out or was canceled.";
        }
        finally
        {
            _loading = false;
        }
    }

    private bool Matches(BoardGame game)
    {
        var players = ParsePlayerCount();

        var nameMatches = string.IsNullOrWhiteSpace(SearchText)
            || game.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase);

        var playersMatch = players is null
            || (game.MinPlayers <= players && players <= game.MaxPlayers);

        return nameMatches && playersMatch;
    }

    private int? ParsePlayerCount() =>
        int.TryParse(PlayerCountInput, out var count) && count > 0
            ? count
            : null;

    private void RollDice()
    {
        var pool = FilteredGames;

        if (pool.Count == 0)
        {
            HighlightedGameId = null;
            return;
        }

        var index = Random.Shared.Next(pool.Count);
        HighlightedGameId = pool[index].Id;
    }

    private void ClearAddedMessage()
    {
        LastAddedMessage = null;
    }

    private Task HandleAddToGameNight(BoardGame game)
    {
        if (!Night.Add(game))
        {
            LastAddedMessage = $"{game.Name} er allerede i spillekvelden.";
            return Task.CompletedTask;
        }

        LastAddedMessage = $"{game.Name} lagt til i spillekvelden.";
        return Task.CompletedTask;
    }
}