using GangsAPI.Data.Gang;
using GangsAPI.Data.Stat;
using GangsAPI.Permissions;

namespace Mock;

public class MockPlayer(ulong steam) : IGangPlayer {
  public ulong Steam { get; } = steam;
  public string? Name { get; init; }
  public int? GangId { get; init; }
}