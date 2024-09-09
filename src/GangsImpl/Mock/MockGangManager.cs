using CounterStrikeSharp.API.Core;
using GangsAPI.Data.Gang;
using GangsAPI.Services;
using GangsAPI.Services.Gang;
using GangsAPI.Services.Player;

namespace Mock;

public class MockGangManager(IPlayerManager playerMgr, IRankManager rankMgr)
  : IGangManager {
  protected readonly HashSet<IGang> CachedGangs = [], BackendGangs = [];
  protected readonly IPlayerManager PlayerMgr = playerMgr;

  public Task<IEnumerable<IGang>> GetGangs() {
    return Task.FromResult(CachedGangs.AsEnumerable());
  }

  public Task<IGang?> GetGang(int id) {
    return Task.FromResult(CachedGangs.FirstOrDefault(g => g.GangId == id));
  }

  public async Task<IGang?> GetGang(ulong steam) {
    var gangId = (await PlayerMgr.GetPlayer(steam))?.GangId;
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

  public virtual async Task<bool> DeleteGang(int id) {
    var members = await PlayerMgr.GetMembers(id);
    foreach (var member in members) {
      member.GangId   = null;
      member.GangRank = null;
      await PlayerMgr.UpdatePlayer(member);
    }

    await rankMgr.DeleteAllRanks(id);

    return CachedGangs.RemoveWhere(g => g.GangId == id) > 0;
  }

  public virtual async Task<IGang?> CreateGang(string name, ulong owner) {
    if (CachedGangs.Any(g => g.Name == name)) return null;
    var id = CachedGangs.Count + 1;
    if (CachedGangs.Any(g => g.GangId == id)) return null;
    var gang   = new MockGang(id, name);
    var player = await PlayerMgr.GetPlayer(owner);
    if (player == null) return null;
    if (player.GangId != null)
      throw new InvalidOperationException(
        $"Attempted to create a gang for {owner} who is already in gang {player.GangId}");
    await rankMgr.AssignDefaultRanks(id);
    player.GangId   = id;
    player.GangRank = 0;
    await PlayerMgr.UpdatePlayer(player);
    CachedGangs.Add(gang);
    BackendGangs.Add(gang);
    return gang.Clone() as IGang;
  }

  public virtual void Start(BasePlugin? plugin, bool hotReload) { }
  public virtual void Dispose() { }

  public virtual void ClearCache() { CachedGangs.Clear(); }

  public virtual Task Load() {
    CachedGangs.UnionWith(BackendGangs);
    return Task.CompletedTask;
  }
}