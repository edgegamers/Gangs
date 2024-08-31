using GangsAPI.Data.Gang;
using GangsAPI.Data.Stat;
using GangsAPI.Permissions;

namespace Mock;

public class MockPlayer(ulong steam) : IGangPlayer {
  public ulong Steam { get; } = steam;
  public string? Name { get; }
  public int? GangId { get; }
  public IGangRank? Rank { get; }
  public ISet<IStat> Stats { get; } = new HashSet<IStat>();
  public ISet<IStat> Perks { get; } = new HashSet<IStat>();
}