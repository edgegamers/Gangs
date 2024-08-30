using GangsAPI.Services;
using GangsAPI.Struct.Stat;

namespace GangsImpl.Implementations.Memory;

public class StatManager : IStatManager {
  private ISet<IStat> stats = new HashSet<IStat>();

  public Task<IEnumerable<IStat>> GetStats() {
    return Task.FromResult<IEnumerable<IStat>>(stats);
  }

  public Task<IStat?> GetStat(string id) {
    return Task.FromResult(stats.FirstOrDefault(stat => stat.StatId == id));
  }

  public Task<bool> RegisterStat(IStat stat) {
    return Task.FromResult(stats.Add(stat));
  }

  public Task<bool> UnregisterStat(string id) {
    throw new NotImplementedException();
  }

  public Task<bool> UpdateStat(IStat stat) {
    throw new NotImplementedException();
  }

  public Task<bool> UpdateStat<K, V>(IStat<K, V> stat) {
    throw new NotImplementedException();
  }
}