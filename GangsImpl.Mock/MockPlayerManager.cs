using GangsAPI.Data.Gang;
using GangsAPI.Services;

namespace GangsImpl.Memory;

public class MockPlayerManager : IPlayerManager {
  private readonly Dictionary<ulong, IGangPlayer> players = new();

  public Task<IGangPlayer?> GetPlayer(ulong steamId) {
    players.TryGetValue(steamId, out var player);
    return Task.FromResult(player);
  }

  public async Task<IGangPlayer> CreatePlayer(ulong steamId,
    string? name = null) {
    var existing = await GetPlayer(steamId);
    if (existing != null) return existing;
    var player = new MockPlayer(steamId);
    players[steamId] = player;
    return player;
  }

  public Task<bool> DeletePlayer(ulong steamId) {
    return Task.FromResult(players.Remove(steamId));
  }

  public void ClearCache() { players.Clear(); }
}