using GangsAPI.Permissions;

namespace Mock;

public class MockRank(string name, int rank, IGangRank.Permissions perms)
  : IGangRank {
  public string Name { get; } = name;
  public int Rank { get; } = rank;
  public IGangRank.Permissions Perms { get; } = perms;
}