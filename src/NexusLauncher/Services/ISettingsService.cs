using System;
using System.Text.Json;

namespace NexusLauncher.Services;

/// <summary>
/// Simple POCO that is serialized to JSON in the user profile folder.
/// </summary>
public record AppSettings
{
    public bool SidebarCollapsed { get; init; } = false;
    public string Theme { get; init; } = "System"; // Dark | Light | System
    public string AccentColor { get; init; } = "#FF007ACC";
    public double WindowWidth { get; init; } = 1280;
    public double WindowHeight { get; init; } = 720;
    public double WindowLeft { get; init; } = double.NaN;
    public double WindowTop { get; init; } = double.NaN;
}

/// <summary>
/// Service responsible for loading / saving the JSON file.
/// </summary>
public interface ISettingsService
{
    AppSettings Settings { get; }
    void Save();
    void Update(Action<AppSettings> updater);
}
