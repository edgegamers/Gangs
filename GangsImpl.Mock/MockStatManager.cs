using GangsAPI.Data.Stat;
using GangsAPI.Services;

namespace GangsImpl.Memory;

public class MockStatManager : IStatManager {
  private readonly HashSet<IStat> stats = [];

  public Task<IEnumerable<IStat>> GetStats() {
    return Task.FromResult<IEnumerable<IStat>>(stats);
  }

  public Task<IStat?> GetStat(string id) {
    return Task.FromResult(stats.FirstOrDefault(stat => stat.StatId == id));
  }

  public async Task<IStat?> CreateStat(string id, string name,
    string? description = null) {
    var stat = await GetStat(id);
    if (stat != null) return stat;
    stat = new MockStat(id, name, description);
    return stat;
  }

  public Task<bool> RegisterStat(IStat stat) {
    return Task.FromResult(stats.Add(stat));
  }

  public Task<bool> UnregisterStat(string id) {
    var matches = stats.Where(stat => stat.StatId == id).ToList();
    foreach (var stat in matches) stats.Remove(stat);
    return Task.FromResult(matches.Count > 0);
  }

  public void ClearCache() { stats.Clear(); }
}