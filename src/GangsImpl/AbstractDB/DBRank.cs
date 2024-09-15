using GangsAPI.Permissions;

namespace GenericDB;

public class DBRank : IGangRank {
  public int GangId { get; init; }
  public required string Name { get; init; }
  public int Rank { get; init; }
  public Perm Permissions { get; set; }

  public int CompareTo(IGangRank? other) {
    return other == null ? 1 : Rank.CompareTo(other.Rank);
  }
}