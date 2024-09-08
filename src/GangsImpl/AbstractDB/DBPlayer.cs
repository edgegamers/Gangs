using GangsAPI.Data.Gang;

namespace GenericDB;

public class DBPlayer : IGangPlayer {
  public ulong Steam { get; init; }
  public string? Name { get; init; }
  public int? GangId { get; set; }
  public int? GangRank { get; set; }
}