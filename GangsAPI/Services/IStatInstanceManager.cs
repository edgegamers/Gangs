using GangsAPI.Struct.Stat;

namespace GangsAPI.Services;

public interface IStatInstanceManager<K> : IStatManager {
  /// <summary>
  ///   Updates an instance of a statistic with the manager.
  /// </summary>
  /// <param name="stat"></param>
  /// <typeparam name="K"></typeparam>
  /// <typeparam name="V"></typeparam>
  /// <returns></returns>
  Task<bool> UpdateStat<V>(IStat<K, V> stat);

  /// <summary>
  ///   Retrieves an instance of a statistic by its key and ID.
  /// </summary>
  /// <param name="key"></param>
  /// <param name="id"></param>
  /// <typeparam name="K"></typeparam>
  /// <typeparam name="V"></typeparam>
  /// <returns></returns>
  Task<IStat<K, V>?> GetStat<V>(K key, string id);
}