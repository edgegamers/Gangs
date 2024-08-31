using GangsAPI.Data.Stat;

namespace GangsImpl.SQL;

public class SQLStat : IStat {
  public string StatId { get; set; }
  public string Name { get; set; }
  public string? Description { get; set; }
}