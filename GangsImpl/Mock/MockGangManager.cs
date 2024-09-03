using GangsAPI.Data.Gang;
using GangsAPI.Services;

namespace Mock;

public class MockGangManager : IGangManager {
  private readonly HashSet<IGang> cachedGangs = [], backendGangs = [];

  public Task<IEnumerable<IGang>> GetGangs() {
    return Task.FromResult(cachedGangs.AsEnumerable());
  }

  public Task<IGang?> GetGang(int id) {
    return Task.FromResult(cachedGangs.FirstOrDefault(g => g.GangId == id));
  }

  public Task<IGang?> GetGang(ulong steam) {
    return Task.FromResult(
      cachedGangs.FirstOrDefault(g => g.Members.ContainsKey(steam)));
  }

  public Task<bool> UpdateGang(IGang gang) {
    var g = cachedGangs.FirstOrDefault(g => g.GangId == gang.GangId);
    if (g == null) return Task.FromResult(false);
    g.Name = gang.Name;
    g.Members.Clear();
    foreach (var member in gang.Members) g.Members.Add(member);
    return Task.FromResult(true);
  }

  public Task<bool> DeleteGang(int id) {
    return Task.FromResult(cachedGangs.RemoveWhere(g => g.GangId == id) > 0);
  }

  public Task<IGang?> CreateGang(string name, ulong owner) {
    var id   = cachedGangs.Count + 1;
    var gang = new MockGang(id, name, owner);
    if (cachedGangs.Any(g => g.GangId == id))
      return Task.FromResult<IGang?>(null);
    cachedGangs.Add(gang);
    backendGangs.Add(gang);
    return Task.FromResult(gang.Clone() as IGang);
  }

  public void ClearCache() { cachedGangs.Clear(); }

  public Task Load() {
    cachedGangs.UnionWith(backendGangs);
    return Task.CompletedTask;
  }
}