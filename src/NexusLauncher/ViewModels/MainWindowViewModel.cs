using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using NexusLauncher.Infrastructure;
using NexusLauncher.Services;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

namespace NexusLauncher.ViewModels;

/// <summary>
/// ViewModel for the MainWindow – handles sidebar commands,
/// window size persistence and toggling the collapsed state.
/// </summary>
public sealed partial class MainWindowViewModel : ObservableObject
{
    private readonly ISettingsService _settings;
    private readonly ILogger<MainWindowViewModel> _logger;
    private readonly ThemeManager _themeManager;
    private readonly NavigationService _nav;

    // UI‑bound properties
    [ObservableProperty] private bool _isSidebarCollapsed;
    [ObservableProperty] private string _currentTheme = "System";

    public ICommand ToggleSidebarCommand { get; }
    public ICommand ChangeThemeCommand { get; }

    public MainWindowViewModel(
        ISettingsService settings,
        ILogger<MainWindowViewModel> logger,
        ThemeManager themeManager,
        NavigationService nav)
    {
        _settings = settings;
        _logger = logger;
        _themeManager = themeManager;
        _nav = nav;

        // Load persisted UI state
        IsSidebarCollapsed = _settings.Settings.SidebarCollapsed;
        CurrentTheme = _settings.Settings.Theme;

        ToggleSidebarCommand = new RelayCommand(ToggleSidebar);
        ChangeThemeCommand = new RelayCommand<string>(ChangeTheme);
    }

    private void ToggleSidebar()
    {
        IsSidebarCollapsed = !IsSidebarCollapsed;
        _settings.Update(s => s = s with { SidebarCollapsed = IsSidebarCollapsed });
        _logger.LogInformation("Sidebar collapsed: {Collapsed}", IsSidebarCollapsed);
    }

    private void ChangeTheme(string themeName)
    {
        if (string.IsNullOrWhiteSpace(themeName)) return;
        CurrentTheme = themeName;
        _theme_manager.SetAndPersist(themeName);
    }

    // Called from the MainWindow code‑behind after the ContentControl is created
    public void NavigateToHome(ContentControl host) => _nav.NavigateToHome(host);
    public void NavigateToSettings(ContentControl host) => _nav.NavigateToSettings(host);

    // Navigation commands – they are set from the MainWindow ctor
    public ICommand? NavigateHomeCommand { get; set; }
    public ICommand? NavigateAllGamesCommand { get; set; }
    public ICommand? NavigateFavoritesCommand { get; set; }
    public ICommand? NavigateSettingsCommand { get; set; }

    // Add Game command (exposed to the toolbar)
    public ICommand? AddGameCommand { get; set; }

    // Sidebar width – persisted together with collapse state
    [ObservableProperty]
    private double _sidebarWidth = 200; // default width
}
