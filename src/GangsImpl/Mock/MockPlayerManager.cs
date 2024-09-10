using GangsAPI.Data.Gang;
using GangsAPI.Services.Player;

namespace Mock;

public class MockPlayerManager : IPlayerManager {
  private readonly Dictionary<ulong, IGangPlayer> players = new();

  public async Task<IGangPlayer?> GetPlayer(ulong steamId, bool create = true) {
    if (players.TryGetValue(steamId, out var player)) return player;
    return create ? await CreatePlayer(steamId) : null;
  }

  public Task<IGangPlayer> CreatePlayer(ulong steamId, string? name = null) {
    var existing = players.GetValueOrDefault(steamId);
    if (existing != null) return Task.FromResult(existing);
    var player = new MockPlayer(steamId) { Name = name };
    players[steamId] = player;
    return Task.FromResult<IGangPlayer>(player);
  }

  public Task<IEnumerable<IGangPlayer>> GetAllPlayers() {
    return Task.FromResult<IEnumerable<IGangPlayer>>(players.Values);
  }

  public Task<IEnumerable<IGangPlayer>> GetMembers(int gangId) {
    return Task.FromResult(players.Values.Where(p => p.GangId == gangId));
  }

  public Task<bool> UpdatePlayer(IGangPlayer player) {
    if (player.GangId == null != (player.GangRank == null))
      throw new InvalidOperationException(
        "Player must have both GangId and GangRank set or neither set");

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