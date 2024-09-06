using GangsAPI.Data;
using GangsAPI.Data.Gang;
using GangsAPI.Data.Stat;

namespace GangsAPI.Services;

public interface IGangStatManager : ICacher {
  Task<bool> GetForGang<TV>(int key, string statId, out TV? holder);
  Task<bool> SetForGang<TV>(int gangId, string statId, TV value);
  Task<bool> RemoveFromGang(int gangId, string statId);

  #region Aliases

  #region Get

  Task<bool> GetForGang<TV>(IGang gang, string statId, out TV? holder) {
    return GetForGang(gang.GangId, statId, out holder);
  }

  async Task<bool> GetForGang<TV>(IGang gang, IStat<TV> holder) {
    var success = await GetForGang(gang, holder.StatId, out TV? tmp);
    if (!success || tmp == null) return false;
    holder.Value = tmp;
    return true;
  }

  Task<bool> GetForGang<TV>(IGang gang, IStat<TV> stat, out TV? holder) {
    return GetForGang(gang, stat.StatId, out holder);
  }

  #endregion

  #region Set

  Task<bool> SetForGang<TV>(int gangId, IStat<TV> stat) {
    return SetForGang(gangId, stat.StatId, stat.Value);
  }

  Task<bool> SetForGang<TV>(IGang gang, IStat<TV> stat) {
    return SetForGang(gang.GangId, stat);
  }

  Task<bool> SetForGang<TV>(IGang gang, string statId, TV value) {
    return SetForGang(gang.GangId, statId, value);
  }

  #endregion

  #region Remove

  Task<bool> RemoveFromGang(int gangId, IStat stat) {
    return RemoveFromGang(gangId, stat.StatId);
  }

  Task<bool> RemoveFromGang(IGang gang, string statId) {
    return RemoveFromGang(gang.GangId, statId);
  }

  Task<bool> RemoveFromGang(IGang gang, IStat stat) {
    return RemoveFromGang(gang, stat.StatId);
  }

  #endregion

  #endregion
}