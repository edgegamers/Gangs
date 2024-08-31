using GangsAPI.Data.Gang;
using GangsAPI.Data.Stat;
using GangsAPI.Permissions;

namespace Mock;

public class MockGang : IGang {
  public MockGang(int id, string name, ulong owner) {
    GangId  = id;
    Name    = name;
    Members = new Dictionary<ulong, IGangRank>();
    Perks   = new HashSet<IStat>();
    Stats   = new HashSet<IStat>();
    Ranks   = new HashSet<IGangRank>();

    Members.Add(owner,
      new MockGangRank(0, "Owner", IGangRank.Permissions.OWNER));
  }

  public int GangId { get; }
  public string Name { get; set; }
  public IDictionary<ulong, IGangRank> Members { get; }
  public ISet<IGangRank> Ranks { get; }
  public ISet<IStat> Perks { get; }
  public ISet<IStat> Stats { get; }

  public object Clone() {
    var clone = new MockGang(GangId, Name, ((IGang)this).Owner);
    clone.Members.Clear();
    foreach (var member in Members) clone.Members.Add(member);
    foreach (var perk in Perks) clone.Perks.Add(perk);
    foreach (var stat in Stats) clone.Stats.Add(stat);
    return clone;
  }
}