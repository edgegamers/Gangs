using GangsAPI.Struct.Stat;

namespace GangsAPI.Services;

public interface IPlayerStatManager {
  Task<IGangStat<V>?> GetForPlayer<V>(ulong key, string id);
  Task<bool> PushToPlayer<V>(ulong key, string id, V value);
}