using CounterStrikeSharp.API.Core;
using GangsAPI.Data.Stat;
using GangsAPI.Services;

namespace Mock;

public class MockStatManager : IStatManager {
  private readonly HashSet<IStat> backendStats = [];
  protected readonly HashSet<IStat> CachedStats = [];

  public virtual void Start(BasePlugin? plugin, bool hotReload) { }

  public Task<IEnumerable<IStat>> GetStats() {
    return Task.FromResult<IEnumerable<IStat>>(CachedStats);
  }

  public Task<IStat?> GetStat(string id) {
    return Task.FromResult(
      CachedStats.FirstOrDefault(stat => stat.StatId == id));
  }

  public virtual async Task<IStat?> CreateStat(string id, string name,
    string? description = null) {
    var stat = await GetStat(id);
    if (stat != null) return stat;
    stat = new MockStat(id, name, description);
    return stat;
  }

  public virtual Task<bool> RegisterStat(IStat stat) {
    if (!CachedStats.Add(stat)) return Task.FromResult(false);
    backendStats.Add(stat);
    return Task.FromResult(true);
  }

  public virtual Task<bool> UnregisterStat(string id) {
    var matches = CachedStats.Where(stat => stat.StatId == id).ToList();
    foreach (var stat in matches) {
      CachedStats.Remove(stat);
      backendStats.Remove(stat);
    }

    return Task.FromResult(matches.Count > 0);
  }

  public void ClearCache() { CachedStats.Clear(); }

  public virtual Task Load() {
    CachedStats.UnionWith(backendStats);
    return Task.CompletedTask;
  }
}