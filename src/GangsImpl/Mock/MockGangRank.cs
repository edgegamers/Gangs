using GangsAPI.Permissions;

namespace Mock;

public class MockGangRank(int rank, string name, Perm perms = 0) : IGangRank {
  public string Name { get; set; } = name;
  public int Rank { get; } = rank;
  public Perm Permissions { get; set; } = perms;

  public int CompareTo(IGangRank? other) {
    return other == null ? 1 : Rank.CompareTo(other.Rank);
  }
}