using System;
using System.Windows;
using Microsoft.Extensions.Logging;
using NexusLauncher.Services;

namespace NexusLauncher.Infrastructure;

/// <summary>
/// Handles runtime theme switching and persisting the choice.
/// </summary>
public sealed class ThemeManager
{
    private readonly ISettingsService _settings;
    private readonly ILogger<ThemeManager> _logger;

    public ThemeManager(ISettingsService settings, ILogger<ThemeManager> logger)
    {
        _settings = settings;
        _logger = logger;
    }

    public void ApplySavedTheme()
    {
        ApplyTheme(_settings.Settings.Theme);
    }

    public void ApplyTheme(string themeName)
    {
        var dict = new ResourceDictionary();
        switch (themeName.ToLowerInvariant())
        {
            case "dark":
                dict.Source = new Uri("pack://application:,,,/NexusLauncher;component/Resources/Themes/Dark.xaml");
                break;
            case "light":
                dict.Source = new Uri("pack://application:,,,/NexusLauncher;component/Resources/Themes/Light.xaml");
                break;
            default:
                // System – use OS colour preference (simple heuristic)
                var isLight = SystemParameters.WindowGlassColor.R > 127;
                dict.Source = new Uri(isLight
                    ? "pack://application:,,,/NexusLauncher;component/Resources/Themes/Light.xaml"
                    : "pack://application:,,,/NexusLauncher;component/Resources/Themes/Dark.xaml");
                break;
        }

        // Remove any existing theme dictionaries
        for (int i = Application.Current.Resources.MergedDictionaries.Count - 1; i >= 0; i--)
        {
            var md = Application.Current.Resources.MergedDictionaries[i];
            if (md.Source?.OriginalString?.Contains("/Themes/") == true)
                Application.Current.Resources.MergedDictionaries.RemoveAt(i);
        }

        Application.Current.Resources.MergedDictionaries.Add(dict);
        _logger.LogInformation("Applied {Theme} theme", themeName);
    }

    public void SetAndPersist(string themeName)
    {
        ApplyTheme(themeName);
        _settings.Update(s => s = s with { Theme = themeName });
    }
}
