using System;
using System.IO;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NexusLauncher.Infrastructure;
using NexusLauncher.Services;

namespace NexusLauncher;

public partial class App : Application
{
    public new static App Current => (App)Application.Current;
    public IServiceProvider Services { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        ConfigureServices();

        // Apply saved theme before any UI shows up
        var themeMgr = Services.GetRequiredService<ThemeManager>();
        themeMgr.ApplySavedTheme();

        // Show the main window
        var mainWindow = Services.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    private void ConfigureServices()
    {
        var services = new ServiceCollection();

        // Logging – console + debug (file logging can be added later)
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.AddDebug();
            builder.SetMinimumLevel(LogLevel.Information);
        });

        // Core services
        services.AddSingleton<IDbService, DbService>();
        services.AddSingleton<ISettingsService, SettingsService>();
        services.AddSingleton<ThemeManager>();
        services.AddSingleton<NavigationService>();

        // ViewModels
        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<HomeViewModel>();
        services.AddTransient<SettingsViewModel>();

        // Views / Windows
        services.AddTransient<MainWindow>();
        services.AddTransient<HomeView>();
        services.AddTransient<SettingsView>();

        Services = services.BuildServiceProvider();

        // Ensure the DB exists before anything else touches it
        Services.GetRequiredService<IDbService>().InitializeDatabase();
    }
}
