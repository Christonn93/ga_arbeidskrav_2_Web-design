using System.Collections.Generic;

/// <summary>
/// In-memory game data store.
/// Registered as a Singleton service.
/// </summary>
public class GameDataStore
{
    // Add a new property to the GameDataStore class.
    private readonly Dictionary<string, string?> _data =
        new Dictionary<string, string?>();
    
    // Add a new property to the GameDataStore class.
    public void Set(string key, string value)
    {
        _data[key] = value;
    }
    
   // Get a property from the GameDataStore class.
    public string? Get(string key, string defaultValue = "")
    {
        return _data.TryGetValue(key, out var value) ? value : defaultValue;
    }
}