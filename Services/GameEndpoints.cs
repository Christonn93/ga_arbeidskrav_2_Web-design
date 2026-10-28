using CafeDiceRoller.Data;
using CafeDiceRoller.Models;
using Microsoft.EntityFrameworkCore;

namespace CafeDiceRoller.Services;

public static class GameEndpoints
{
    // Registers all CRUD endpoints under /api/games.
    public static IEndpointRouteBuilder MapGameEndpoints(
        this IEndpointRouteBuilder app)
    {
        var games = app.MapGroup("/api/games");
        
        // READ: all games
        games.MapGet("", async (GameDbContext db) =>
        {
            var result = await db.Set<BoardGame>()
                .AsNoTracking()
                .ToListAsync();

            return Results.Ok(result);
        });

        // READ: one game
        games.MapGet("/{id:int}", async (
            int id,
            GameDbContext db) =>
        {
            var game = await db.Set<BoardGame>().FindAsync(id);

            if (game is null)
            {
                return Results.NotFound();
            }

            return Results.Ok(game);
        });

        // CREATE
        games.MapPost("", async (
            BoardGame game,
            GameDbContext db) =>
        {
            // Let the database generate the ID.
            game.Id = 0;

            db.Set<BoardGame>().Add(game);
            await db.SaveChangesAsync();

            return Results.Created(
                $"/api/games/{game.Id}",
                game);
        });

        // UPDATE
        games.MapPut("/{id:int}", async (
            int id,
            BoardGame updatedGame,
            GameDbContext db) =>
        {
            // Ensure route id and body id match.
            if (id != updatedGame.Id)
            {
                return Results.BadRequest(new
                {
                    message = "The route ID must match the game ID."
                });
            }

            var existingGame = await db.Set<BoardGame>()
                .FindAsync(id);

            if (existingGame is null)
            {
                return Results.NotFound();
            }

            // Overwrite tracked values with the incoming game.
            db.Entry(existingGame)
                .CurrentValues
                .SetValues(updatedGame);

            await db.SaveChangesAsync();

            return Results.NoContent();
        });

        // DELETE
        games.MapDelete("/{id:int}", async (
            int id,
            GameDbContext db) =>
        {
            var game = await db.Set<BoardGame>().FindAsync(id);

            if (game is null)
            {
                return Results.NotFound();
            }

            db.Set<BoardGame>().Remove(game);
            await db.SaveChangesAsync();

            return Results.NoContent();
        });

        return app;
    }
}