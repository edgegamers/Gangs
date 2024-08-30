using GangsAPI.Struct.Stat;

namespace GangsAPI.Services;

public interface IGangStatManager {
  Task<IGangStat<V>?> GetForGang<V>(int key, string id);
  Task<bool> PushToGang<V>(int key, string id, V value);
}