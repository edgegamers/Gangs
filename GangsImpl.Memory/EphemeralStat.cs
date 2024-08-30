using GangsAPI.Struct.Stat;

namespace GangsImpl.Memory;

public class EphemeralStat(string statId, string name, string? desc) : IStat {
  public string StatId { get; } = statId;
  public string Name { get; } = name;
  public string? Description { get; } = desc;
}