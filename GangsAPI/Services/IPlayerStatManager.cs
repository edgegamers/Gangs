using GangsAPI.Data;
using GangsAPI.Data.Stat;

namespace GangsAPI.Services;

public interface IPlayerStatManager : IPluginBehavior, ICacher {
  Task<bool> GetForPlayer<TV>(ulong steam, string statId, out TV? result);
  Task<bool> SetForPlayer<TV>(ulong steam, string statId, TV value);
  Task<bool> RemoveFromPlayer(ulong steam, string statId);

  #region Aliases

  #region Get

  Task<bool> GetForPlayer<TV>(PlayerWrapper wrapper, string statId,
    out TV? holder) {
    return GetForPlayer(wrapper.Steam, statId, out holder);
  }

  async Task<bool> GetForPlayer<TV>(ulong steam, IStat<TV> holder) {
    var success = await GetForPlayer(steam, holder.StatId, out TV? tmp);
    if (!success || tmp == null) return false;
    holder.Value = tmp;
    return true;
  }

  Task<bool> GetForPlayer<TV>(PlayerWrapper wrapper, IStat<TV> holder) {
    return GetForPlayer(wrapper.Steam, holder);
  }

  #endregion

  #region Set

  Task<bool> SetForPlayer<TV>(ulong steam, IStat<TV> stat) {
    return SetForPlayer(steam, stat.StatId, stat.Value);
  }

  Task<bool> SetForPlayer<TV>(PlayerWrapper wrapper, IStat<TV> stat) {
    return SetForPlayer(wrapper.Steam, stat);
  }

  Task<bool> SetForPlayer<TV>(PlayerWrapper wrapper, string statId, TV value) {
    return SetForPlayer(wrapper.Steam, statId, value);
  }

  #endregion

  #region Remove

  Task<bool> RemoveFromPlayer(ulong steam, IStat stat) {
    return RemoveFromPlayer(steam, stat.StatId);
  }

  Task<bool> RemoveFromPlayer(PlayerWrapper wrapper, string statId) {
    return RemoveFromPlayer(wrapper.Steam, statId);
  }

  Task<bool> RemoveFromPlayer(PlayerWrapper wrapper, IStat stat) {
    return RemoveFromPlayer(wrapper, stat.StatId);
  }

  #endregion

  #endregion
}