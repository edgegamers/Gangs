using GangsAPI.Struct;
using GangsAPI.Struct.Stat;

namespace GangsAPI.Services;

/// <summary>
/// A manager for statistics. Allows for the registration, retrieval, and updating of statistics.
/// </summary>
public interface IStatManager : IPluginBehavior {
  /// <summary>
  /// Retrieves all statistics.
  /// </summary>
  /// <returns></returns>
  Task<IEnumerable<IStat>> GetStats();

  /// <summary>
  /// Retrieves a statistic by its ID.
  /// </summary>
  /// <param name="id"></param>
  /// <returns></returns>
  Task<IStat?> GetStat(string id);

  /// <summary>
  /// Registers a statistic with the manager.
  /// </summary>
  /// <param name="stat"></param>
  /// <returns></returns>
  Task<bool> RegisterStat(IStat stat);

  /// <summary>
  /// Unregisters a statistic with the manager.
  /// </summary>
  /// <param name="id"></param>
  /// <returns></returns>
  Task<bool> UnregisterStat(string id);

  /// <summary>
  /// Updates a statistic with the manager.
  /// </summary>
  /// <param name="stat"></param>
  /// <returns></returns>
  Task<bool> UpdateStat(IStat stat);

  /// <summary>
  /// Updates an instance of a statistic with the manager.
  /// </summary>
  /// <param name="stat"></param>
  /// <typeparam name="K"></typeparam>
  /// <typeparam name="V"></typeparam>
  /// <returns></returns>
  Task<bool> UpdateStat<K, V>(IStat<K, V> stat);
}