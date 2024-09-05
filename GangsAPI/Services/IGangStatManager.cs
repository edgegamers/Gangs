using GangsAPI.Data;
using GangsAPI.Data.Gang;
using GangsAPI.Data.Stat;

namespace GangsAPI.Services;

public interface IGangStatManager : ICacher {
  Task<IStat<V>?> GetForGang<V>(int key, string id);
  Task<IStat?> GetForGang(int key, string id);
  Task<bool> PushToGang<V>(int gangId, string id, V value);

  Task<IStat?> GetForGang(IGang gang, string id) {
    return GetForGang(gang.GangId, id);
  }

  Task<IStat<V>?> GetForGang<V>(IGang gang, string id) {
    return GetForGang<V>(gang.GangId, id);
  }

  Task<bool> PushToGang<V>(IGang gang, string id, V value) {
    return PushToGang(gang.GangId, id, value);
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

  Task<bool> PushToGang<V>(int gangId, IStat stat, V value) {
    return PushToGang(gangId, stat.StatId, value);
  }

  Task<IStat?> GetForGang(IGang gang, IStat stat) {
    return GetForGang(gang, stat.StatId);
  }

  Task<IStat<V>?> GetForGang<V>(IGang gang, IStat<V> stat) {
    return GetForGang<V>(gang, stat.StatId);
  }
}