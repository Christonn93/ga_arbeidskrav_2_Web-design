using System.Net.Http.Json;
using CafeDiceRoller.Models;

namespace CafeDiceRoller.Services;

// Register with AddHttpClient<GameApiClient>, not AddSingleton.
// BaseAddress must end with '/' so these relative paths resolve correctly.
public sealed class GameApiClient(HttpClient http)
{
    // Fetches all games from the API.
    public async Task<List<BoardGame>> GetAllAsync()
    {
        // ?? [] hides a null body (204 or bad payload).
        return await http.GetFromJsonAsync<List<BoardGame>>("api/games") ?? [];
    }

    // Creates a game and returns the server version.
    public async Task<BoardGame> CreateAsync(BoardGame game)
    {
        // Sending BoardGame posts Id and other server-owned fields.
        var response = await http.PostAsJsonAsync("api/games", game);

        // Fail instead of returning a null game on empty body.
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<BoardGame>()
               ?? throw new InvalidOperationException("API returned an empty game.");
    }

    // Updates an existing game by id.
    public async Task UpdateAsync(BoardGame game)
    {
        // Same over-posting issue as create. A dedicated update DTO is safer.
        var response = await http.PutAsJsonAsync($"api/games/{game.Id}", game);
        response.EnsureSuccessStatusCode();
    }

    // Deletes a game by id.
    public async Task DeleteAsync(int id)
    {
        var response = await http.DeleteAsync($"api/games/{id}");
        response.EnsureSuccessStatusCode();
    }

    // Shared helper so every method surfaces the API error body.
    private static async Task EnsureSuccessAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode) return;
        var body = await response.Content.ReadAsStringAsync();
        throw new HttpRequestException(
            $"API {(int)response.StatusCode} {response.ReasonPhrase}: {body}",
            null,
            response.StatusCode);
    }
}