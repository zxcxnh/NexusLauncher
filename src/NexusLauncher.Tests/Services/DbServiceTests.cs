using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NexusLauncher.Models;
using NexusLauncher.Services;
using Xunit;

namespace NexusLauncher.Tests.Services;

public class DbServiceTests : IDisposable
{
    private readonly IDbService _db;
    private readonly string _dbPath;

    public DbServiceTests()
    {
        var logger = Mock.Of<ILogger<DbService>>();
        _db = new DbService(logger);
        // Grab the private field to delete the file later
        var field = typeof(DbService).GetField("_dbPath", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
        _dbPath = (string)field.GetValue(_db)!;
    }

    [Fact]
    public async Task Add_And_Retrieve_Game()
    {
        var game = new Game
        {
            Name = "Test Game",
            ExecutablePath = @"C:\Games\TestGame\game.exe",
            WorkingDirectory = @"C:\Games\TestGame",
            LaunchArguments = "",
            IsFavorite = false
        };

        await _db.AddGameAsync(game);
        var all = await _db.GetAllGamesAsync();

        var retrieved = Assert.Single(all);
        Assert.Equal(game.Name, retrieved.Name);
        Assert.Equal(game.ExecutablePath, retrieved.ExecutablePath);
    }

    [Fact]
    public async Task Update_Game()
    {
        var game = new Game
        {
            Name = "Original",
            ExecutablePath = @"C:\Orig\orig.exe",
            WorkingDirectory = @"C:\Orig",
            IsFavorite = false
        };
        await _db.AddGameAsync(game);

        game.Name = "Renamed";
        game.IsFavorite = true;
        await _db.UpdateGameAsync(game);

        var all = await _db.GetAllGamesAsync();
        var updated = Assert.Single(all);
        Assert.Equal("Renamed", updated.Name);
        Assert.True(updated.IsFavorite);
    }

    [Fact]
    public async Task Delete_Game()
    {
        var game = new Game { Name = "ToDelete", ExecutablePath = @"C:\tmp\del.exe" };
        await _db.AddGameAsync(game);
        await _db.DeleteGameAsync(game.Id);

        var all = await _db.GetAllGamesAsync();
        Assert.Empty(all);
    }

    public void Dispose()
    {
        try { if (File.Exists(_dbPath)) File.Delete(_dbPath); } catch { }
    }
}
