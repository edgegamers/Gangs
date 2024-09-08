using GangsAPI.Data;
using GangsAPI.Data.Gang;
using GangsAPI.Data.Stat;

namespace GangsAPI.Services.Gang;

public interface IGangStatManager : IPluginBehavior {
  Task<(bool, TV?)> GetForGang<TV>(int key, string statId);
  Task<bool> SetForGang<TV>(int gangId, string statId, TV value);
  Task<bool> RemoveFromGang(int gangId, string statId);

  #region Aliases

  #region Get

  Task<(bool, TV?)> GetForGang<TV>(IGang gang, string statId) {
    return GetForGang<TV>(gang.GangId, statId);
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