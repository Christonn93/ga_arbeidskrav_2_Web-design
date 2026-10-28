using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using CafeDiceRoller.Models;
using CafeDiceRoller.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace CafeDiceRoller.Components.Pages;

public partial class AdminGames
{
    [Inject]
    private GameApiClient GameClient { get; set; } = default!;

    [Inject]
    private ILoggerFactory LoggerFactory { get; set; } = default!;

    [Inject]
    private IWebHostEnvironment HostEnvironment { get; set; } = default!;

    private readonly List<BoardGame> _games = [];

    private GameFormModel _form = new();

    private int? _editingId;

    private BoardGame? _pendingDelete;

    private bool _showForm;

    private bool _showDelete;

    private bool _loading = true;

    private string? _loadError;

    private string? _saveError;

    protected override async Task OnInitializedAsync()
    {
        await Load();
    }

    private async Task Load()
    {
        _loading = true;
        _loadError = null;

        var logger = LoggerFactory.CreateLogger("GamesAdmin");

        try
        {
            var games = await GameClient.GetAllAsync();

            _games.Clear();
            _games.AddRange(games);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Failed to load games.");

            _loadError = HostEnvironment.EnvironmentName == "Development"
                ? ex.ToString()
                : "Could not load games. Please try again later.";
        }
        catch (TaskCanceledException ex)
        {
            logger.LogError(
                ex,
                "Loading games was canceled or timed out."
            );

            _loadError = "The games request timed out or was canceled.";
        }
        finally
        {
            _loading = false;
        }
    }

    private void OpenCreate()
    {
        ResetForm();
        _showForm = true;
    }

    private void Edit(BoardGame game)
    {
        _editingId = game.Id;
        _saveError = null;

        _form = new GameFormModel
        {
            Id = game.Id,
            Name = game.Name,
            Category = game.Category,
            MinPlayers = game.MinPlayers,
            MaxPlayers = game.MaxPlayers,
            PlayTimeMinutes = game.PlayTimeMinutes,
            Rating = game.Rating,
            Description = game.Description
        };

        _showForm = true;
    }

    private void CloseForm()
    {
        _showForm = false;
        ResetForm();
    }

    private async Task Save(GameFormModel model)
    {
        _saveError = null;

        var game = ToGame(model);

        try
        {
            if (_editingId is null)
            {
                var created = await GameClient.CreateAsync(game);

                _games.Add(created);
            }
            else
            {
                game.Id = _editingId.Value;

                await GameClient.UpdateAsync(game);

                var index = _games.FindIndex(
                    item => item.Id == game.Id
                );

                if (index >= 0)
                {
                    _games[index] = game;
                }
            }

            _showForm = false;
            ResetForm();
        }
        catch (HttpRequestException)
        {
            _saveError = "Could not save the game.";
        }
    }

    private void AskDelete(BoardGame game)
    {
        _pendingDelete = game;
        _showDelete = true;
    }

    private void CancelDelete()
    {
        _showDelete = false;
        _pendingDelete = null;
    }

    private async Task ConfirmDelete()
    {
        if (_pendingDelete is null)
        {
            return;
        }

        var id = _pendingDelete.Id;

        try
        {
            await GameClient.DeleteAsync(id);

            _games.RemoveAll(game => game.Id == id);

            if (_editingId == id)
            {
                ResetForm();
            }

            _showDelete = false;
            _pendingDelete = null;
        }
        catch (HttpRequestException)
        {
            _saveError = "Could not delete the game.";
        }
    }

    private void ResetForm()
    {
        _editingId = null;
        _saveError = null;
        _form = new GameFormModel();
    }

    private static BoardGame ToGame(GameFormModel model) => new()
    {
        Id = model.Id,
        Name = model.Name,
        Category = model.Category,
        MinPlayers = model.MinPlayers,
        MaxPlayers = model.MaxPlayers,
        PlayTimeMinutes = model.PlayTimeMinutes,
        Rating = model.Rating,
        Description = model.Description
    };
}