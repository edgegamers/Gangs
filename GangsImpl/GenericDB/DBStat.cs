using GangsAPI.Data.Stat;

namespace GenericDB;

/// <summary>
///   Dapper-compatible representation of a stat.
/// </summary>
public class DBStat(string StatID, string Name, string? Description) : IStat {
  public string StatId { get; init; } = StatID;
  public string Name { get; init; } = Name;
  public string? Description { get; init; } = Description;
}