using GangsAPI.Data.Gang;
using GangsAPI.Data.Stat;
using GangsAPI.Permissions;
using Mock;

namespace GenericDB;

/// <summary>
///   Dapper-compatible representation of a gang.
/// </summary>
public class DBGang : MockGang {
  public DBGang(int id, string name, ulong owner,
    IDictionary<ulong, IGangRank> members, ISet<IGangRank> ranks,
    ISet<IStat> perks, ISet<IStat> stats) : base(id, name, owner) {
    Members = members;
    Ranks   = ranks;
    Perks   = perks;
    Stats   = stats;
  }

  public DBGang(IGang gang) : base(gang.GangId, gang.Name, gang.Owner) {
    Members = gang.Members;
    Ranks   = gang.Ranks;
    Perks   = gang.Perks;
    Stats   = gang.Stats;
  }

  public DBGang(int id, string name, ulong owner) : base(id, name, owner) { }
}