using GangsAPI.Data.Gang;
using GangsAPI.Data.Stat;
using GangsAPI.Permissions;

namespace Mock;

public class MockGang : IGang {
  public MockGang(int id, string name, ulong owner) {
    GangId  = id;
    Name    = name;
    Members = new Dictionary<ulong, IGangRank>();
    Ranks   = new HashSet<IGangRank>();

    var ownerRank = new MockGangRank(0, "Owner", IGangRank.Permissions.OWNER);

    Members.Add(owner, ownerRank);
    Ranks.Add(ownerRank);
  }

  public int GangId { get; protected init; }
  public string Name { get; set; }
  public IDictionary<ulong, IGangRank> Members { get; protected init; }
  public ISet<IGangRank> Ranks { get; protected init; }

  public object Clone() {
    var clone = new MockGang(GangId, Name, ((IGang)this).Owner);
    clone.Members.Clear();
    foreach (var member in Members) clone.Members.Add(member);
    return clone;
  }
}