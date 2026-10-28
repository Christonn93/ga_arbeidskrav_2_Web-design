using System.Collections.Generic;

/// <summary>
/// In-memory game data store.
/// Registered as a Singleton service.
/// </summary>
public class GameDataStore
{
    // Backing dictionary for key/value data.
    private readonly Dictionary<string, string?> _data =
        new Dictionary<string, string?>();

    // Stores or overwrites a value for the given key.
    public void Set(string key, string value)
    {
        _data[key] = value;
    }

    // Returns the value for a key, or a default if missing.
    public string? Get(string key, string defaultValue = "")
    {
        return _data.TryGetValue(key, out var value) ? value : defaultValue;
    }
}