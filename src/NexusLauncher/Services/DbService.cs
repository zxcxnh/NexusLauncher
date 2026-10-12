using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
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

        SqlMapper.AddTypeHandler(new GuidTypeHandler());

        // Use an isolated temp DB during xUnit runs so tests do not share state across runs
        if (IsRunningUnderTest())
        {
            _dbPath = Path.Combine(Path.GetTempPath(), $"nexus_test_{Guid.NewGuid()}.db");
        }
        else
        {
            var folder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "NexusLauncher");
            Directory.CreateDirectory(folder);
            _dbPath = Path.Combine(folder, DbFileName);
        }

        _connection = new SqliteConnection($"Data Source={_dbPath}");
        _connection.Open();

        InitializeDatabase();
    }

    private static bool IsRunningUnderTest()
    {
        try
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Any(a => (a.FullName ?? string.Empty).Contains("xunit", StringComparison.OrdinalIgnoreCase));
        }
        catch
        {
            return false;
        }
    }

    public void InitializeDatabase()
    {
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

        try
        {
            if (IsRunningUnderTest() && File.Exists(_dbPath))
                File.Delete(_dbPath);
        }
        catch
        {
            // ignore cleanup failures
        }
    }
}

public sealed class GuidTypeHandler : SqlMapper.TypeHandler<Guid>
{
    public override Guid Parse(object value)
    {
        if (value is Guid g) return g;
        if (value is string s) return Guid.Parse(s);
        return Guid.Parse(value.ToString()!);
    }

    public override void SetValue(IDbDataParameter parameter, Guid value)
    {
        parameter.Value = value.ToString();
        parameter.DbType = DbType.String;
    }
}
