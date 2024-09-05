using GangsAPI.Data;
using GangsAPI.Data.Gang;
using GangsAPI.Data.Stat;

namespace GangsAPI.Services;

public interface IGangStatManager : ICacher {
  Task<IStat<V>?> GetForGang<V>(int key, string id);
  Task<IStat?> GetForGang(int key, string id);

  Task<IStat?> GetForGang(IGang gang, string id) {
    return GetForGang(gang.GangId, id);
  }

  Task<IStat<V>?> GetForGang<V>(IGang gang, string id) {
    return GetForGang<V>(gang.GangId, id);
  }

  Task<bool> PushToGang<V>(int gangId, IStat<V> stat);

  Task<bool> PushToGang<V>(IGang gang, IStat<V> stat) {
    return PushToGang(gang.GangId, stat);
  }

  Task<IStat?> GetForGang(IGang gang, IStat stat) {
    return GetForGang(gang, stat.StatId);
  }

  Task<IStat<V>?> GetForGang<V>(IGang gang, IStat<V> stat) {
    return GetForGang<V>(gang, stat.StatId);
  }
}