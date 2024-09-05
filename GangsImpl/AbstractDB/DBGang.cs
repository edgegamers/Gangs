using GangsAPI.Data.Gang;
using GangsAPI.Permissions;
using Mock;

namespace GenericDB;

/// <summary>
///   Dapper-compatible representation of a gang.
/// </summary>
public class DBGang : MockGang {
  public DBGang(int id, string name, ulong owner,
    IDictionary<ulong, IGangRank> members, ISet<IGangRank> ranks) : base(id,
    name, owner) {
    Members = members;
    Ranks   = ranks;
  }

  public DBGang(IGang gang) : base(gang.GangId, gang.Name, gang.Owner) {
    Members = gang.Members;
    Ranks   = gang.Ranks;
  }

  public DBGang(int id, string name, ulong owner) : base(id, name, owner) { }
}