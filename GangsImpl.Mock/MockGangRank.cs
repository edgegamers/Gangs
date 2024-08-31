using GangsAPI.Permissions;

namespace GangsImpl.Memory;

public class MockGangRank(int rank, string name, IGangRank.Permissions perms = 0) : IGangRank {
  public string Name { get; } = name;
  public int Rank { get; } = rank;
  public IGangRank.Permissions Perms { get; } = perms;
}