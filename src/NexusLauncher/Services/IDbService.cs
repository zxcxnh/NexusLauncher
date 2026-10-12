using System.Collections.Generic;
using System.Threading.Tasks;
using NexusLauncher.Models;

namespace NexusLauncher.Services;

/// <summary>
/// Very small data‑access contract used by the UI and by tests.
/// </summary>
public interface IDbService
{
    void InitializeDatabase();
    Task<IReadOnlyList<Game>> GetAllGamesAsync();
    Task AddGameAsync(Game game);
    Task UpdateGameAsync(Game game);
    Task DeleteGameAsync(Guid id);
}
