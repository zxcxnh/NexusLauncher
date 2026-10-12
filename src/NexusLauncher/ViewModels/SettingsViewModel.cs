using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using NexusLauncher.Infrastructure;
using NexusLauncher.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace NexusLauncher.ViewModels;

/// <summary>
/// Settings page – lets the user pick a theme and toggle a few UI bits.
/// </summary>
public sealed partial class SettingsViewModel : ObservableObject
{
    private readonly ISettingsService _settings;
    private readonly ThemeManager _themeMgr;
    private readonly ILogger<SettingsViewModel> _logger;

    public ObservableCollection<string> AvailableThemes { get; } = new()
    {
        "System",
        "Light",
        "Dark"
    };

    [ObservableProperty] private string _selectedTheme = "System";

    public ICommand ApplyThemeCommand { get; }

    public SettingsViewModel(ISettingsService settings, ThemeManager themeMgr, ILogger<SettingsViewModel> logger)
    {
        _settings = settings;
        _themeMgr = themeMgr;
        _logger = logger;

        SelectedTheme = _settings.Settings.Theme;
        ApplyThemeCommand = new RelayCommand(ApplyTheme);
    }

    private void ApplyTheme()
    {
        _themeMgr.SetAndPersist(SelectedTheme);
        _logger.LogInformation("User changed theme to {Theme}", SelectedTheme);
    }
}
