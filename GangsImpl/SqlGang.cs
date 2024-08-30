using GangsAPI;
using GangsAPI.Permissions;
using GangsAPI.Struct;

namespace GangsImpl;

public class SqlGang : IGang {
  public int Id { get; }
  public string Name { get; set; }
  public IDictionary<ulong, IGangRank> Members { get; }
  public int Bank { get; set; }
  public ISet<IPerk> Perks { get; }
  public ISet<IStat> Stats { get; }

  public SqlGang() { }
}