using GangsAPI.Data.Stat;

namespace GenericDB;

/// <summary>
///   Dapper-compatible representation of a stat.
/// </summary>
public class DBStat : IStat {
  public string StatId { get; set; }
  public string Name { get; set; }
  public string? Description { get; set; }
}