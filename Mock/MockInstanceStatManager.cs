using GangsAPI.Data.Stat;
using GangsAPI.Services;

namespace GangsImpl.Memory;

public class MockInstanceStatManager(IStatManager mgr)
  : IPlayerStatManager, IGangStatManager {
  private readonly Dictionary<int, Dictionary<string, IStat>> gangStats = [];

  private readonly Dictionary<ulong, Dictionary<string, IStat>>
    playerStats = [];

  public Task<IGangStat<V>?> GetForGang<V>(int key, string id) {
    if (!gangStats.TryGetValue(key, out var gangStatMap))
      return Task.FromResult<IGangStat<V>?>(null);
    if (!gangStatMap.TryGetValue(id, out var result))
      return Task.FromResult<IGangStat<V>?>(null);
    return Task.FromResult(result as IGangStat<V>);
  }

  public async Task<bool> PushToGang<V>(int gangId, string id, V value) {
    if (!gangStats.TryGetValue(gangId, out var gangStatMap))
      gangStats[gangId] = gangStatMap = new Dictionary<string, IStat>();
    var stat = await mgr.GetStat(id);
    if (stat == null) return false;
    gangStatMap[id] = new MockGangStat<V>(stat, value);
    return true;
  }

  public Task<IPlayerStat<V>?> GetForPlayer<V>(ulong key, string id) {
    if (!playerStats.TryGetValue(key, out var playerStatMap))
      return Task.FromResult<IPlayerStat<V>?>(null);
    if (!playerStatMap.TryGetValue(id, out var result))
      return Task.FromResult<IPlayerStat<V>?>(null);
    return Task.FromResult(result as IPlayerStat<V>);
  }

  public async Task<bool> PushToPlayer<V>(ulong key, string id, V value) {
    if (!playerStats.TryGetValue(key, out var playerStatMap))
      playerStats[key] = playerStatMap = new Dictionary<string, IStat>();
    var stat = await mgr.GetStat(id);
    if (stat == null) return false;
    playerStatMap[id] = new MockPlayerStat<V>(stat, value);
    return true;
  }

  public void ClearCache() {
    gangStats.Clear();
    playerStats.Clear();
  }

  public Task Load() { return Task.CompletedTask; }
}