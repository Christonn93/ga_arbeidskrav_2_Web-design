using CafeDiceRoller.Models;
using Microsoft.AspNetCore.Components;

namespace CafeDiceRoller.Components.Ui.Card;

public partial class GameCard
{
    // Required board game to display on the card.
    [Parameter, EditorRequired]
    public BoardGame Game { get; set; } = default!;

    // Highlights the card when picked by the dice.
    [Parameter]
    public bool Highlighted { get; set; }

    // Indicates whether the game is already added to game night.
    [Parameter]
    public bool IsInGameNight { get; set; }

    // Raised when the user adds the game to game night.
    [Parameter]
    public EventCallback<BoardGame> OnAddToGameNight { get; set; }

    // Adds the game to game night if not already added.
    private async Task HandleAddToNight()
    {
        if (IsInGameNight)
        {
            return;
        }

        await OnAddToGameNight.InvokeAsync(Game);
    }
}