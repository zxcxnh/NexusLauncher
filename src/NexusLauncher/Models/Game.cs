using System;

namespace NexusLauncher.Models;

/// <summary>
/// Minimal representation of a game. Additional fields (metadata, artwork, etc.)
/// will be added in later phases.
/// </summary>
public sealed class Game
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string ExecutablePath { get; set; } = string.Empty; // full path to .exe
    public string WorkingDirectory { get; set; } = string.Empty;
    public string LaunchArguments { get; set; } = string.Empty;
    public DateTime DateAdded { get; set; } = DateTime.UtcNow;
    public bool IsFavorite { get; set; } = false;
}
