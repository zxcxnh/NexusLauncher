using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using NexusLauncher.Models;

namespace NexusLauncher.Services;

/// <summary>
/// Simple wrapper around SQLite + Dapper. Handles DB creation, migrations,
/// and CRUD for the Game table. All statements are parameterised – no SQL‑injection risk.
/// </summary>
public sealed class DbService : IDbService, IDisposable
{
    private const string DbFileName = "nexus.db";
    private readonly string _dbPath;
    private readonly ILogger<DbService> _logger;
    private readonly SqliteConnection _connection;

    public DbService(ILogger<DbService> logger)
    {
        _logger = logger;

        var folder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "NexusLauncher");
        Directory.CreateDirectory(folder);
        _dbPath = Path.Combine(folder, DbFileName);

        _connection = new SqliteConnection($"Data Source={_dbPath}");
        _connection.Open();

        // Ensure migrations / tables are present immediately after construction
        InitializeDatabase();
    }

    public void InitializeDatabase()
    {
        // Simple migration – create table if it does not exist.
        const string createTable = @"
            CREATE TABLE IF NOT EXISTS Games (
                Id               TEXT PRIMARY KEY,
                Name             TEXT NOT NULL,
                ExecutablePath   TEXT NOT NULL,
                WorkingDirectory TEXT,
                LaunchArguments  TEXT,
                DateAdded        TEXT NOT NULL,
                IsFavorite       INTEGER NOT NULL
            );";

        _connection.Execute(createTable);
        _logger.LogInformation("Database initialized at {Path}", _dbPath);
    }

    public async Task<IReadOnlyList<Game>> GetAllGamesAsync()
    {
        const string sql = "SELECT * FROM Games ORDER BY Name COLLATE NOCASE;";
        var result = await _connection.QueryAsync<Game>(sql);
        return result.AsList();
    }

    public async Task AddGameAsync(Game game)
    {
        const string sql = @"
            INSERT INTO Games (Id, Name, ExecutablePath, WorkingDirectory, LaunchArguments, DateAdded, IsFavorite)
            VALUES (@Id, @Name, @ExecutablePath, @WorkingDirectory, @LaunchArguments, @DateAdded, @IsFavorite);";

        await _connection.ExecuteAsync(sql, new
        {
            Id = game.Id.ToString(),
            game.Name,
            game.ExecutablePath,
            game.WorkingDirectory,
            game.LaunchArguments,
            DateAdded = game.DateAdded.ToString("o"),
            IsFavorite = game.IsFavorite ? 1 : 0
        });
        _logger.LogInformation("Added game {Name} ({Id})", game.Name, game.Id);
    }

    public async Task UpdateGameAsync(Game game)
    {
        const string sql = @"
            UPDATE Games
            SET Name = @Name,
                ExecutablePath = @ExecutablePath,
                WorkingDirectory = @WorkingDirectory,
                LaunchArguments = @LaunchArguments,
                IsFavorite = @IsFavorite
            WHERE Id = @Id;";

        await _connection.ExecuteAsync(sql, new
        {
            Id = game.Id.ToString(),
            game.Name,
            game.ExecutablePath,
            game.WorkingDirectory,
            game.LaunchArguments,
            IsFavorite = game.IsFavorite ? 1 : 0
        });
        _logger.LogInformation("Updated game {Name} ({Id})", game.Name, game.Id);
    }

    public async Task DeleteGameAsync(Guid id)
    {
        const string sql = "DELETE FROM Games WHERE Id = @Id;";
        await _connection.ExecuteAsync(sql, new { Id = id.ToString() });
        _logger.LogInformation("Deleted game {Id}", id);
    }

    public void Dispose()
    {
        _connection?.Dispose();
    }
}
