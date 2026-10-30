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
    // HTTP client for the games API.
    [Inject]
    private GameApiClient GameClient { get; set; } = default!;

    // Factory used to create loggers.
    [Inject]
    private ILoggerFactory LoggerFactory { get; set; } = default!;

    // Provides environment name for dev error details.
    [Inject]
    private IWebHostEnvironment HostEnvironment { get; set; } = default!;

    // Cached list of games loaded from the API.
    private readonly List<BoardGame> _games = [];

    // Form model for create/edit operations.
    private GameFormModel _form = new();

    // Id of the game being edited, null when creating.
    private int? _editingId;

    // Game pending deletion confirmation.
    private BoardGame? _pendingDelete;

    // Controls visibility of the form popup.
    private bool _showForm;

    // Controls visibility of the delete popup.
    private bool _showDelete;

    // Indicates an in-flight load request.
    private bool _loading = true;

    // Load error message to display, if any.
    private string? _loadError;

    // Save/delete error message to display, if any.
    private string? _saveError;

    // Loads games when the page is first rendered.
    protected override async Task OnInitializedAsync()
    {
        await Load();
    }

    // Fetches all games from the API and handles errors.
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
            logger.LogError(ex, "Loading games was canceled or timed out.");

            _loadError = "The games request timed out or was canceled.";
        }
        finally
        {
            _loading = false;
        }
    }

    // Opens the form in create mode.
    private void OpenCreate()
    {
        ResetForm();
        _showForm = true;
    }

    // Opens the form populated with the selected game.
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

    // Closes the form and resets local state.
    private void CloseForm()
    {
        _showForm = false;
        ResetForm();
    }

    // Creates or updates a game based on current mode.
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

                var index = _games.FindIndex(item => item.Id == game.Id);

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

    // Opens the delete confirmation popup.
    private void AskDelete(BoardGame game)
    {
        _pendingDelete = game;
        _showDelete = true;
    }

    // Cancels the delete confirmation.
    private void CancelDelete()
    {
        _showDelete = false;
        _pendingDelete = null;
    }

    // Deletes the pending game via the API.
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

    // Resets form state back to create mode.
    private void ResetForm()
    {
        _editingId = null;
        _saveError = null;
        _form = new GameFormModel();
    }

    // Maps a form model to a BoardGame instance.
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