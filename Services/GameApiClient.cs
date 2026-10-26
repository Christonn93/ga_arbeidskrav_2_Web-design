using System.Net.Http.Json;
using CafeDiceRoller.Models;

namespace CafeDiceRoller.Services;

// Register with AddHttpClient<GameApiClient>, not AddSingleton.
// BaseAddress must end with '/' so these relative paths resolve correctly.
public sealed class GameApiClient(HttpClient http)
{
    // Pass a shared JsonSerializerOptions if the API is camelCase or uses custom names.
    // Add CancellationToken so Blazor/ASP.NET can cancel the call.
    public async Task<List<BoardGame>> GetAllAsync()
    {
        // ?? [] hides a null body (204 or a bad payload). Log that case if the UI looks empty for no reason.
        // Prefer IReadOnlyList<BoardGame> if callers should not mutate the result.
        // This loads every game; page the API if the catalog can grow.
        return await http.GetFromJsonAsync<List<BoardGame>>("api/games") ?? [];
    }

    public async Task<BoardGame> CreateAsync(BoardGame game)
    {
        // Sending BoardGame posts Id and other server-owned fields. Prefer a create DTO without Id.
        var response = await http.PostAsJsonAsync("api/games", game);

        // EnsureSuccessStatusCode drops the response body. Read it and include it in the exception.
        response.EnsureSuccessStatusCode();

        // Good: fail instead of returning a null game when the body is empty.
        return await response.Content.ReadFromJsonAsync<BoardGame>()
               ?? throw new InvalidOperationException("API returned an empty game.");
    }

    public async Task UpdateAsync(BoardGame game)
    {
        // Same over-posting issue as create. A dedicated update DTO is safer.
        // If the API returns the saved entity (timestamps, row version), return BoardGame like CreateAsync.
        var response = await http.PutAsJsonAsync($"api/games/{game.Id}", game);
        response.EnsureSuccessStatusCode();

        // 404 and 412 (concurrency) are different from 500. Map them if the UI must reload or show "not found".
    }

    public async Task DeleteAsync(int id)
    {
        var response = await http.DeleteAsync($"api/games/{id}");
        response.EnsureSuccessStatusCode();

        // 404 may mean "already deleted". Decide whether that is success or an error for the caller.
    }

    // Shared helper so every method surfaces the API error body, not only the status code:
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