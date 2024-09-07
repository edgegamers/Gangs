using CounterStrikeSharp.API.Core;
using GangsAPI.Data.Gang;
using GangsAPI.Services;
using GangsAPI.Services.Gang;
using GangsAPI.Services.Player;

namespace Mock;

public class MockGangManager(IPlayerManager playerMgr) : IGangManager {
  protected readonly HashSet<IGang> CachedGangs = [], BackendGangs = [];

  public Task<IEnumerable<IGang>> GetGangs() {
    return Task.FromResult(CachedGangs.AsEnumerable());
  }

  public Task<IGang?> GetGang(int id) {
    return Task.FromResult(CachedGangs.FirstOrDefault(g => g.GangId == id));
  }

  public async Task<IGang?> GetGang(ulong steam) {
    var gangId = (await playerMgr.GetPlayer(steam))?.GangId;
    return gangId == null ?
      null :
      CachedGangs.FirstOrDefault(g => g.GangId == gangId);
  }

  public virtual Task<bool> UpdateGang(IGang gang) {
    var g = CachedGangs.FirstOrDefault(g => g.GangId == gang.GangId);
    if (g == null) return Task.FromResult(false);
    g.Name = gang.Name;
    return Task.FromResult(true);
  }

  public virtual Task<bool> DeleteGang(int id) {
    return Task.FromResult(CachedGangs.RemoveWhere(g => g.GangId == id) > 0);
  }

  public virtual async Task<IGang?> CreateGang(string name, ulong owner) {
    var id     = CachedGangs.Count + 1;
    var gang   = new MockGang(id, name);
    var player = await playerMgr.GetPlayer(owner, true);
    if (player == null) return null;
    player.GangId = id;
    await playerMgr.UpdatePlayer(player);
    if (CachedGangs.Any(g => g.GangId == id)) return null;
    CachedGangs.Add(gang);
    BackendGangs.Add(gang);
    return gang.Clone() as IGang;
  }

  public virtual void ClearCache() { CachedGangs.Clear(); }

  public virtual Task Load() {
    CachedGangs.UnionWith(BackendGangs);
    return Task.CompletedTask;
  }

  public virtual void Start(BasePlugin? plugin, bool hotReload) { }
  public virtual void Dispose() { }
}