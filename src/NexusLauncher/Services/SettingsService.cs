using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace NexusLauncher.Services;

/// <summary>
/// Service responsible for loading / saving the JSON file.
/// </summary>
public sealed class SettingsService : ISettingsService
{
    private const string SettingsFileName = "settings.json";
    private readonly string _settingsPath;
    private readonly ILogger<SettingsService> _logger;
    private AppSettings _settings;

    public SettingsService(ILogger<SettingsService> logger)
    {
        _logger = logger;

        var folder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "NexusLauncher");

        Directory.CreateDirectory(folder);
        _settingsPath = Path.Combine(folder, SettingsFileName);

        _settings = LoadOrCreate();
    }

    public AppSettings Settings => _settings;

    public void Save()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_settingsPath)!);
            var json = JsonSerializer.Serialize(_settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_settingsPath, json);
            _logger.LogInformation("Settings saved to {Path}", _settingsPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save settings");

            // Surface the real problem in tests instead of swallowing it
            if (IsRunningUnderTest())
                throw;
        }
    }

    public void Update(Action<AppSettings> updater)
    {
        var copy = _settings with { };
        updater(copy);
        _settings = copy;
        Save();
    }

    private AppSettings LoadOrCreate()
    {
        try
        {
            if (File.Exists(_settingsPath))
            {
                var json = File.ReadAllText(_settingsPath);
                var loaded = JsonSerializer.Deserialize<AppSettings>(json);
                if (loaded != null)
                {
                    _settings = loaded;
                    return loaded;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not read settings – using defaults");
        }

        var defaults = new AppSettings();
        _settings = defaults;
        Save();
        return defaults;
    }

    private static bool IsRunningUnderTest()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => (a.FullName ?? string.Empty).Contains("xunit", StringComparison.OrdinalIgnoreCase));
    }
}
