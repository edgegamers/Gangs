using GangsAPI.Data;
using GangsAPI.Data.Stat;

namespace GangsAPI.Services;

/// <summary>
///   A manager for statistics. Allows for the registration, retrieval, and updating of statistics.
/// </summary>
public interface IStatManager : IPluginBehavior {
  /// <summary>
  ///   Retrieves all statistics.
  /// </summary>
  /// <returns></returns>
  Task<IEnumerable<IStat>> GetStats();

  /// <summary>
  ///   Retrieves a statistic by its ID.
  /// </summary>
  /// <param name="id"></param>
  /// <returns></returns>
  Task<IStat?> GetStat(string id);

  /// <summary>
  ///   Creates a statistic with the manager, but does not register it.
  ///   If the statistic already exists with the same ID,
  ///   it will return the existing statistic.
  /// </summary>
  /// <param name="id"></param>
  /// <param name="name"></param>
  /// <param name="description"></param>
  /// <returns></returns>
  Task<IStat?> CreateStat(string id, string name, string? description = null);

  /// <summary>
  ///   Registers a statistic with the manager.
  /// </summary>
  /// <param name="stat"></param>
  /// <returns></returns>
  Task<bool> RegisterStat(IStat stat);

  /// <summary>
  ///   Unregisters a statistic with the manager.
  /// </summary>
  /// <param name="id"></param>
  /// <returns></returns>
  Task<bool> UnregisterStat(string id);
}