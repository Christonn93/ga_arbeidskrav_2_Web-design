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
    // A plain List is enough for now. We only add/remove single games and read
    // the whole list, so there is no need for anything more advanced.
    private readonly List<BoardGame> _games = new();
    
    // Fired every time the list changes. Components that care (the header badge, the game-night page, the shelf) 
    // subscribe to this and call StateHasChanged, so the UI updates at once.
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
    
    // Add a game. Returns false if it is already there, so the caller can show a message instead. 
    // We check by Id because the same game must not be added twice.
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
    
    /// Remove a game by id. Returns false if it was not there in the first place, so the caller knows nothing changed.
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

    // Small helper so I don't forget the null check when raising the event.
    // If nobody has subscribed, OnChange is null and we just do nothing.
    private void NotifyStateChanged() => OnChange?.Invoke();
}
