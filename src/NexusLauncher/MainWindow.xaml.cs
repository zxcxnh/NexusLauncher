using System;
using System.ComponentModel;
using System.Windows;
using Microsoft.Extensions.Logging;
using NexusLauncher.Services;
using NexusLauncher.ViewModels;
using CommunityToolkit.Mvvm.Input;

namespace NexusLauncher;

/// <summary>
/// Handles window‑size persistence and forwards the ContentControl to the
/// navigation service via the ViewModel.
/// </summary>
public partial class MainWindow : Window
{
    private readonly ISettingsService _settings;
    private readonly ILogger<MainWindow> _logger;
    public MainWindowViewModel MainWindowVM { get; }

    public MainWindow(MainWindowViewModel vm,
                      ISettingsService settings,
                      ILogger<MainWindow> logger)
    {
        InitializeComponent();
        DataContext = MainWindowVM = vm;
        _settings = settings;
        _logger = logger;

        // Hook navigation commands that live in the VM
        MainWindowVM.NavigateHomeCommand = new RelayCommand(() => MainWindowVM.NavigateToHome(ContentHost));
        MainWindowVM.NavigateAllGamesCommand = new RelayCommand(() => { /* later */ });
        MainWindowVM.NavigateFavoritesCommand = new RelayCommand(() => { /* later */ });
        MainWindowVM.NavigateSettingsCommand = new RelayCommand(() => MainWindowVM.NavigateToSettings(ContentHost));
        MainWindowVM.AddGameCommand = new RelayCommand(() =>
        {
            // Forward to the HomeViewModel (the Home view is always loaded at startup)
            if (ContentHost.Content is Views.HomeView home)
            {
                var vm = (HomeViewModel)home.DataContext;
                vm.AddGameCommand.Execute(null);
            }
        });
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        // Restore size/position (if saved)
        var s = _settings.Settings;
        if (!double.IsNaN(s.WindowLeft) && !double.IsNaN(s.WindowTop))
        {
            Left = s.WindowLeft;
            Top = s.WindowTop;
        }

        Width = s.WindowWidth;
        Height = s.WindowHeight;

        // Initial navigation
        MainWindowVM.NavigateToHome(ContentHost);
    }

    private void Window_Closing(object? sender, CancelEventArgs e)
    {
        // Persist size/position
        _settings.Update(s => s = s with
        {
            WindowWidth = Width,
            WindowHeight = Height,
            WindowLeft = Left,
            WindowTop = Top
        });
    }
}
