using GangsAPI.Data.Gang;

namespace Mock;

public class MockPlayer(ulong steam) : IGangPlayer {
  public ulong Steam { get; } = steam;
  public string? Name { get; set; }
  public int? GangId { get; set; }
  public int? GangRank { get; set; }
}