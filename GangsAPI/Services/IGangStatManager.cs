using GangsAPI.Data;
using GangsAPI.Data.Gang;
using GangsAPI.Data.Stat;

namespace GangsAPI.Services;

public interface IGangStatManager : ICacher {
  Task<IGangStat<V>?> GetForGang<V>(int key, string id);
  Task<bool> PushToGang<V>(int gangId, string id, V value);

  Task<IGangStat<V>?> GetForGang<V>(IGang gang, string id) {
    return GetForGang<V>(gang.GangId, id);
  }

  Task<bool> PushToGang<V>(IGang gang, string id, V value) {
    return PushToGang(gang.GangId, id, value);
  }

  Task<IGangStat<V>?> GetForGang<V>(IGang gang, IStat stat, V value) {
    return GetForGang<V>(gang, stat.StatId);
  }

  Task<bool> PushToGang<V>(IGang gang, IStat stat, V value) {
    return PushToGang(gang, stat.StatId, value);
  }

  Task<bool> PushToGang<V>(int gangId, IStat<V> stat) {
    return PushToGang(gangId, stat.StatId, stat.Value);
  }

  Task<bool> PushToGang<V>(IGang gang, IStat<V> stat) {
    return PushToGang(gang, stat.StatId, stat.Value);
  }

  Task<IGangStat<V>?> GetForGang<V>(int key, IStat stat, V value) {
    return GetForGang<V>(key, stat.StatId);
  }

  Task<bool> PushToGang<V>(int gangId, IStat stat, V value) {
    return PushToGang(gangId, stat.StatId, value);
  }

  Task<IGangStat<V>?> GetForGang<V>(IGang gang, IStat stat) {
    return GetForGang<V>(gang, stat.StatId);
  }
}