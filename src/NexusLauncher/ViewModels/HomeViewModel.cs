using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using NexusLauncher.Models;
using NexusLauncher.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;

namespace NexusLauncher.ViewModels;

/// <summary>
/// Home screen – shows the library (empty at Phase 1) and lets the user add a game.
/// </summary>
public sealed partial class HomeViewModel : ObservableObject
{
    private readonly IDbService _db;
    private readonly ILogger<HomeViewModel> _logger;

    public ObservableCollection<Game> Games { get; } = new();

    public ICommand AddGameCommand { get; }

    public HomeViewModel(IDbService db, ILogger<HomeViewModel> logger)
    {
        _db = db;
        _logger = logger;
        AddGameCommand = new AsyncRelayCommand(AddGameAsync);
        // Load existing games (if any) – fire‑and‑forget
        _ = LoadGamesAsync();
    }

    private async Task LoadGamesAsync()
    {
        var list = await _db.GetAllGamesAsync();
        Application.Current.Dispatcher.Invoke(() =>
        {
            Games.Clear();
            foreach (var g in list) Games.Add(g);
        });
    }

    private async Task AddGameAsync()
    {
        // Simple OpenFileDialog to pick an .exe – later we can expand to a full wizard
        var dlg = new OpenFileDialog
        {
            Filter = "Executable files (*.exe)|*.exe",
            Title = "Select the game's main executable"
        };

        if (dlg.ShowDialog() != true) return;

        var exePath = dlg.FileName;
        var name = System.IO.Path.GetFileNameWithoutExtension(exePath);

        var game = new Game
        {
            Name = name,
            ExecutablePath = exePath,
            WorkingDirectory = System.IO.Path.GetDirectoryName(exePath) ?? string.Empty
        };

        await _db.AddGameAsync(game);
        Games.Add(game);
        _logger.LogInformation("Manually added game {Name}", name);
    }
}
