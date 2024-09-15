using GangsAPI.Permissions;

namespace Mock;

public class MockRank(string name, int rank, Perm perms) : IGangRank {
  public string Name { get; } = name;
  public int Rank { get; } = rank;
  public Perm Permissions { get; set; } = perms;

  public int CompareTo(IGangRank? other) {
    return other == null ? 1 : Rank.CompareTo(other.Rank);
  }
}