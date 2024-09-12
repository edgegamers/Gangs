using GangsAPI.Data.Stat;

namespace GangsAPI.Services;

/// <summary>
///   A manager for statistics. Allows for the registration, retrieval, and updating of statistics.
/// </summary>
public interface IStatManager : IPluginBehavior {
  IEnumerable<IStat> Stats { get; }
}