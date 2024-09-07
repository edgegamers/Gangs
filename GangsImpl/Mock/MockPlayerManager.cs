using GangsAPI.Data.Gang;
using GangsAPI.Services.Player;

namespace Mock;

public class MockPlayerManager : IPlayerManager {
  private readonly Dictionary<ulong, IGangPlayer> players = new();

  public async Task<IGangPlayer?> GetPlayer(ulong steamId, bool create = true) {
    if (players.TryGetValue(steamId, out var player)) return player;
    return await (create ? CreatePlayer(steamId) : null)!;
  }

  public Task<IGangPlayer> CreatePlayer(ulong steamId, string? name = null) {
    var existing = players.GetValueOrDefault(steamId);
    if (existing != null) return Task.FromResult(existing);
    var player = new MockPlayer(steamId) { Name = name };
    players[steamId] = player;
    return Task.FromResult<IGangPlayer>(player);
  }

  public Task<bool> UpdatePlayer(IGangPlayer player) {
    if (!players.ContainsKey(player.Steam)) return Task.FromResult(false);
    players[player.Steam] = player;
    return Task.FromResult(true);
  }

  public Task<bool> DeletePlayer(ulong steamId) {
    return Task.FromResult(players.Remove(steamId));
  }

  public void ClearCache() { players.Clear(); }

  public Task Load() { return Task.CompletedTask; }
}