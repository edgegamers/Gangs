using GangsAPI.Data.Stat;
using GangsAPI.Services;

namespace Mock;

public class MockInstanceStatManager(IStatManager mgr)
  : IPlayerStatManager, IGangStatManager {
  private readonly Dictionary<int, Dictionary<string, IStat>> gangStats = [];

  private readonly Dictionary<ulong, Dictionary<string, IStat>>
    playerStats = [];

  public Task<IStat<V>?> GetForGang<V>(int key, string id) {
    if (!gangStats.TryGetValue(key, out var gangStatMap))
      return Task.FromResult<IStat<V>?>(null);
    if (!gangStatMap.TryGetValue(id, out var result))
      return Task.FromResult<IStat<V>?>(null);
    return Task.FromResult(result as IStat<V>);
  }

  public Task<IStat?> GetForGang(int key, string id) {
    if (!gangStats.TryGetValue(key, out var gangStatMap))
      return Task.FromResult<IStat?>(null);
    if (!gangStatMap.TryGetValue(id, out var result))
      return Task.FromResult<IStat?>(null);
    return Task.FromResult(result)!;
  }

  public async Task<bool> PushToGang<V>(int gangId, IStat<V> stat) {
    var registered = await mgr.GetStat(stat.StatId);
    if (registered == null) return false;
    if (!gangStats.TryGetValue(gangId, out var gangStatMap))
      gangStats[gangId] = gangStatMap = new Dictionary<string, IStat>();
    gangStatMap[stat.StatId] = stat.Clone();
    gangStats[gangId]        = gangStatMap;
    return true;
  }

  public Task<IStat<V>?> GetForPlayer<V>(ulong key, string id) {
    if (!playerStats.TryGetValue(key, out var playerStatMap))
      return Task.FromResult<IStat<V>?>(null);
    if (!playerStatMap.TryGetValue(id, out var result))
      return Task.FromResult<IStat<V>?>(null);
    return Task.FromResult(result as IStat<V>);
  }

  public async Task<bool> PushToPlayer<V>(ulong key, string id, V value) {
    if (!playerStats.TryGetValue(key, out var playerStatMap))
      playerStats[key] = playerStatMap = new Dictionary<string, IStat>();
    var stat = await mgr.GetStat(id);
    if (stat == null) return false;
    playerStatMap[id] = new MockStat<V>(stat, value);
    playerStats[key]  = playerStatMap;
    return true;
  }

  public void ClearCache() {
    gangStats.Clear();
    playerStats.Clear();
  }

  public Task Load() { return Task.CompletedTask; }
}