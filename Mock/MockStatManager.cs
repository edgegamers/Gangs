using GangsAPI.Data.Stat;
using GangsAPI.Services;

namespace GangsImpl.Memory;

public class MockStatManager : IStatManager {
  private readonly HashSet<IStat> cachedStats = [], backendStats = [];

  public Task<IEnumerable<IStat>> GetStats() {
    return Task.FromResult<IEnumerable<IStat>>(cachedStats);
  }

  public Task<IStat?> GetStat(string id) {
    return Task.FromResult(
      cachedStats.FirstOrDefault(stat => stat.StatId == id));
  }

  public async Task<IStat?> CreateStat(string id, string name,
    string? description = null) {
    var stat = await GetStat(id);
    if (stat != null) return stat;
    stat = new MockStat(id, name, description);
    return stat;
  }

  public Task<bool> RegisterStat(IStat stat) {
    if (!cachedStats.Add(stat)) return Task.FromResult(false);
    backendStats.Add(stat);
    return Task.FromResult(true);
  }

  public Task<bool> UnregisterStat(string id) {
    var matches = cachedStats.Where(stat => stat.StatId == id).ToList();
    foreach (var stat in matches) {
      cachedStats.Remove(stat);
      backendStats.Remove(stat);
    }

    return Task.FromResult(matches.Count > 0);
  }

  public void ClearCache() { cachedStats.Clear(); }

  public Task Load() {
    cachedStats.UnionWith(backendStats);
    return Task.CompletedTask;
  }
}