using System.Net.Http;
using System.Threading.Tasks;
using CafeDiceRoller.Models;
using CafeDiceRoller.Services;
using Microsoft.AspNetCore.Components;

namespace CafeDiceRoller.Components.Pages;

public partial class GameDetails : IDisposable
{
    // Injected API client for game lookups.
    [Inject]
    private GameApiClient GameClient { get; set; } = default!;

    // Inject Night state
    [Inject]
    private GameNightState Night { get; set; } = default!;

    // Used to navigate from code (the back button).
    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    // Game id from the route parameter.
    [Parameter]
    public int Id { get; set; }

    // Currently resolved game, null if not found.
    private BoardGame? Game { get; set; }

    // Local rating state for the dice input.
    private int LocalRating { get; set; } = 1;

    // True while the game is being fetched from the API.
    private bool _loading = true;

    // Set when the API call fails; otherwise null.
    private string? _loadError;

    // True while a rating change is being saved to the API.
    private bool _savingRating;

    // Set when saving the rating fails; otherwise null.
    private string? _ratingError;

    // Tracks whether the game is added to game night.
    protected override void OnInitialized() => Night.OnChange += StateHasChanged;
    public void Dispose() => Night.OnChange -= StateHasChanged;

    // Id of the last game loaded, used to reset state on route change.
    private int? _loadedGameId;

    // Loads the game from the API and resets local state when Id changes.
    protected override async Task OnParametersSetAsync()
    {
        if (_loadedGameId == Id)
        {
            return;
        }

        _loadedGameId = Id;
        await Load();
    }

    // Fetches the game for the current Id and resets the local rating.
    private async Task Load()
    {
        _loading = true;
        _loadError = null;
        Game = null;

        try
        {
            Game = await GameClient.GetByIdAsync(Id);
            LocalRating = Game?.Rating ?? 1;
        }
        catch (HttpRequestException)
        {
            _loadError = "Could not load the game.";
        }
        catch (TaskCanceledException)
        {
            _loadError = "The game request timed out or was canceled.";
        }
        finally
        {
            _loading = false;
        }
    }

    // Adds the game to game night (placeholder state only).
    private void HandleAddToNight()
    {
        if (Game is null) return;
        Night.Add(Game);   // returns false if already there; harmless
    }

    // Navigates back to the game shelf from code.
    private void GoBack() => Navigation.NavigateTo("/games");

    // Handles a dice roll: updates local state and saves the rating via the API.
    private async Task HandleRatingChanged(int rating)
    {
        LocalRating = rating;

        if (Game is null)
        {
            return;
        }

        _ratingError = null;
        _savingRating = true;

        try
        {
            Game.Rating = rating;
            await GameClient.UpdateAsync(Game);
        }
        catch (HttpRequestException)
        {
            _ratingError = "Kunne ikke lagre vurderingen.";
        }
        catch (TaskCanceledException)
        {
            _ratingError = "Lagringen ble avbrutt eller tok for lang tid.";
        }
        finally
        {
            _savingRating = false;
        }
    }
}
