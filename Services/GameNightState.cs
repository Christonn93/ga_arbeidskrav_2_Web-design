using CafeDiceRoller.Models;

namespace CafeDiceRoller.Services;

/// <summary>
/// Keeps track of the games the guest has picked for tonight.
///
/// This is an application state, not a local state: it has to survive when the user
/// navigates between /games, /games/{id} and /game-night. Because it is
/// registered as a Scoped service, it lives for the whole user session (one
/// SignalR circuit), so the pages can just inject it instead of passing the
/// list around through parameters.
/// </summary>
public class GameNightState
{
    // Holds the selected games in insertion order.
    private readonly List<BoardGame> _games = new();

    // Fired when the list changes; subscribers re-render.
    public event Action? OnChange;

    // The games currently in the game night, in the order they were added.
    public IReadOnlyList<BoardGame> Games => _games;

    // How many games are selected. Used by the badge in the menu.
    public int Count => _games.Count;

    // Total playing time for all selected games. I sum this in the service
    // (not in the component), so the page only has to display the number.
    public int TotalPlayTimeMinutes => _games.Sum(g => g.PlayTimeMinutes);

    // True if a game with this id is already in the game night.
    public bool Contains(int gameId) => _games.Any(g => g.Id == gameId);

    // Adds a game; returns false if it's already present.
    public bool Add(BoardGame game)
    {
        if (Contains(game.Id))
        {
            return false;
        }

        _games.Add(game);
        NotifyStateChanged();
        return true;
    }

    // Removes a game by id; returns false if not found.
    public bool Remove(int gameId)
    {
        var game = _games.FirstOrDefault(g => g.Id == gameId);
        if (game is null)
        {
            return false;
        }

        _games.Remove(game);
        NotifyStateChanged();
        return true;
    }

    // Raises the change event if anyone is subscribed.
    private void NotifyStateChanged() => OnChange?.Invoke();
}