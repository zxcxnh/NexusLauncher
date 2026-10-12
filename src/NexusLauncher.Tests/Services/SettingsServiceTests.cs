using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Moq;
using NexusLauncher.Services;
using Xunit;

namespace NexusLauncher.Tests.Services;

public class SettingsServiceTests
{
    private readonly ISettingsService _svc;
    private readonly string _settingsPath;

    public SettingsServiceTests()
    {
        var logger = Mock.Of<ILogger<SettingsService>>();
        _svc = new SettingsService(logger);

        // Resolve the internal path via reflection (only for test cleanup)
        var field = typeof(SettingsService).GetField("_settingsPath", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
        _settingsPath = (string)field.GetValue(_svc)!;
    }

    [Fact]
    public void SettingsFile_IsCreated_WithDefaults()
    {
        Assert.True(File.Exists(_settingsPath));

        var json = File.ReadAllText(_settingsPath);
        var settings = System.Text.Json.JsonSerializer.Deserialize<AppSettings>(json);
        Assert.NotNull(settings);
        Assert.Equal("System", settings!.Theme);
    }

    [Fact]
    public void Update_ChangesPersisted()
    {
        _svc.Update(s => s = s with { Theme = "Dark", WindowWidth = 1600 });

        var json = File.ReadAllText(_settingsPath);
        var settings = System.Text.Json.JsonSerializer.Deserialize<AppSettings>(json);
        Assert.NotNull(settings);
        Assert.Equal("Dark", settings!.Theme);
        Assert.Equal(1600, settings.WindowWidth);
    }

    // Cleanup after all tests
    ~SettingsServiceTests()
    {
        try { if (File.Exists(_settingsPath)) File.Delete(_settingsPath); } catch { }
    }
}
