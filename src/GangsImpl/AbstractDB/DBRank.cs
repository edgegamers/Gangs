using GangsAPI.Permissions;

namespace GenericDB;

public class DBRank : IGangRank {
  public int GangId { get; init; }
  public string Name { get; init; }
  public int Rank { get; init; }
  public IGangRank.Permissions Perms { get; init; }

  public int CompareTo(IGangRank? other) {
    return other == null ? 1 : Rank.CompareTo(other.Rank);
  }
}