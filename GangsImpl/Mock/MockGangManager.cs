using CounterStrikeSharp.API.Core;
using GangsAPI.Data.Gang;
using GangsAPI.Services;

namespace Mock;

public class MockGangManager : IGangManager {
  protected readonly HashSet<IGang> CachedGangs = [], BackendGangs = [];

  public Task<IEnumerable<IGang>> GetGangs() {
    return Task.FromResult(CachedGangs.AsEnumerable());
  }

  public Task<IGang?> GetGang(int id) {
    return Task.FromResult(CachedGangs.FirstOrDefault(g => g.GangId == id));
  }

  public Task<IGang?> GetGang(ulong steam) {
    return Task.FromResult(
      CachedGangs.FirstOrDefault(g => g.Members.ContainsKey(steam)));
  }

  public virtual Task<bool> UpdateGang(IGang gang) {
    var g = CachedGangs.FirstOrDefault(g => g.GangId == gang.GangId);
    if (g == null) return Task.FromResult(false);
    g.Name = gang.Name;
    g.Members.Clear();
    foreach (var member in gang.Members) g.Members.Add(member);
    return Task.FromResult(true);
  }

  public virtual Task<bool> DeleteGang(int id) {
    return Task.FromResult(CachedGangs.RemoveWhere(g => g.GangId == id) > 0);
  }

  public virtual Task<IGang?> CreateGang(string name, ulong owner) {
    var id   = CachedGangs.Count + 1;
    var gang = new MockGang(id, name, owner);
    if (CachedGangs.Any(g => g.GangId == id))
      return Task.FromResult<IGang?>(null);
    CachedGangs.Add(gang);
    BackendGangs.Add(gang);
    return Task.FromResult(gang.Clone() as IGang);
  }

  public virtual void ClearCache() { CachedGangs.Clear(); }

  public virtual Task Load() {
    CachedGangs.UnionWith(BackendGangs);
    return Task.CompletedTask;
  }

  public virtual void Start(BasePlugin? plugin, bool hotReload) { }
  public virtual void Dispose() { }
}