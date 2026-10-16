using CafeDiceRoller.Models;

namespace CafeDiceRoller.Services;

/// <summary>
/// In-memory game catalog. Registered as a Scoped service.
/// In Oppgave 4 this is replaced by the REST API; <see cref="GameApiClient"/> becomes
/// the source of truth, and this class can be deleted or kept as fallback data.
/// </summary>
public class GameData
{
    private readonly List<BoardGame> _games = new()
    {
        new BoardGame
        {
            Id = 1, Name = "Catan", MinPlayers = 3, MaxPlayers = 4,
            PlayTimeMinutes = 90, Category = "Strategy", Rating = 5,
            Description = "Settlere bygger bosetninger, veier og byer på øya Catan. Handleressurser og forhandle deg til seier."
        },
        new BoardGame
        {
            Id = 2, Name = "Ticket to Ride", MinPlayers = 2, MaxPlayers = 5,
            PlayTimeMinutes = 60, Category = "Family", Rating = 5,
            Description = "Bygg jernbaner mellom byer i USA. Samle kort, krev ruter, og blokker motstanderne."
        },
        new BoardGame
        {
            Id = 3, Name = "Codenames", MinPlayers = 4, MaxPlayers = 8,
            PlayTimeMinutes = 30, Category = "Party", Rating = 4,
            Description = "To lag konkurrerer om å finne spioner ved hjelp av ettords hint. Enkel regel, vanskelig mesterskap."
        },
        new BoardGame
        {
            Id = 4, Name = "Azul", MinPlayers = 2, MaxPlayers = 4,
            PlayTimeMinutes = 45, Category = "Abstract", Rating = 5,
            Description = "Velg fliser, bygg vegger, og samle poeng. Enkelt å lære, vanskelig å mestre."
        },
        new BoardGame
        {
            Id = 5, Name = "Pandemic", MinPlayers = 2, MaxPlayers = 4,
            PlayTimeMinutes = 45, Category = "Cooperative", Rating = 4,
            Description = "Samarbeid om å stoppe sykdomsutbrudd før verden går under. Alle vinner eller alle taper."
        },
        new BoardGame
        {
            Id = 6, Name = "Carcassonne", MinPlayers = 2, MaxPlayers = 5,
            PlayTimeMinutes = 45, Category = "Tile-laying", Rating = 4,
            Description = "Legg brikker, bygg byer og veier, og plasser undersåtter for å score poeng."
        },
        new BoardGame
        {
            Id = 7, Name = "Dixit", MinPlayers = 3, MaxPlayers = 6,
            PlayTimeMinutes = 30, Category = "Party", Rating = 4,
            Description = "Fortell en historie med et billedkort. Pekerne gjør det vanskelig."
        },
        new BoardGame
        {
            Id = 8, Name = "7 Wonders", MinPlayers = 3, MaxPlayers = 7,
            PlayTimeMinutes = 45, Category = "Card drafting", Rating = 5,
            Description = "Bygg opp en sivilisasjon gjennom tre epoker. Draft kort og balanser militær, vitenskap og kultur."
        }
    };

    /// <summary>All games in the collection.</summary>
    public IReadOnlyList<BoardGame> GetAll() => _games;

    /// <summary>One game by id, or null if not found.</summary>
    public BoardGame? GetById(int id) => _games.FirstOrDefault(g => g.Id == id);

    /// <summary>
    /// Filter by optional name substring (case-insensitive) and optional player count.
    /// A game matches the player filter when <paramref name="playerCount"/> lies
    /// within its [MinPlayers, MaxPlayers] range. Pass null to skip a filter.
    /// </summary>
    public IReadOnlyList<BoardGame> Search(string? name, int? playerCount)
    {
        IEnumerable<BoardGame> query = _games;

        if (!string.IsNullOrWhiteSpace(name))
        {
            var needle = name.Trim();
            query = query.Where(g =>
                g.Name.Contains(needle, StringComparison.OrdinalIgnoreCase));
        }

        if (playerCount is { } n)
        {
            query = query.Where(g => g.SupportsPlayerCount(n));
        }

        return query.ToList();
    }
}