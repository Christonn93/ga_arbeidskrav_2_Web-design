using CafeDiceRoller.Models;
using CafeDiceRoller.Services;
using Microsoft.AspNetCore.Components;

namespace CafeDiceRoller.Components.Pages;

public partial class GameDetails
{
    // Injected data source for game lookups.
    [Inject]
    private GameData GameData { get; set; } = default!;

    // Inject Night state
    [Inject]
    private GameNightState Night { get; set; } = default!;
    
    // Game id from the route parameter.
    [Parameter]
    public int Id { get; set; }

    // Currently resolved game, null if not found.
    private BoardGame? Game { get; set; }

    // Local rating state for the dice input.
    private int LocalRating { get; set; } = 1;

    // Tracks whether the game is added to game night.
    protected override void OnInitialized() => Night.OnChange += StateHasChanged;
    public void Dispose() => Night.OnChange -= StateHasChanged;

    // Id of the last game loaded, used to reset state on route change.
    private int? _loadedGameId;

    // Loads the game and resets local state when Id changes.
    protected override void OnParametersSet()
    {
        Game = GameData.GetById(Id);

        if (_loadedGameId == Id)
        {
            return;
        }

        _loadedGameId = Id;
        LocalRating = Game?.Rating ?? 1;
    }

    // Adds the game to game night (placeholder state only).
    private void HandleAddToNight()
    {
        if (Game is null) return;
        Night.Add(Game);   // returns false if already there; harmless
    }
}