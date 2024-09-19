using GangsAPI.Services.Gang;
using GangsAPI.Services.Player;

namespace Mock;

public class MockInstanceStatManager : IPlayerStatManager, IGangStatManager {
  public void ClearCache() {
    cachedGangValues.Clear();
    cachedPlayerValues.Clear();
  }

  public Task Load() {
    cachedGangValues.Clear();
    cachedPlayerValues.Clear();
    foreach (var (gangId, gangStatMap) in backendGangValues)
      cachedGangValues[gangId] = new Dictionary<string, object>(gangStatMap);
    foreach (var (steam, playerStatMap) in backendPlayerValues)
      cachedPlayerValues[steam] = new Dictionary<string, object>(playerStatMap);
    return Task.CompletedTask;
  }

  #region Player

  private readonly Dictionary<ulong, Dictionary<string, object>>
    cachedPlayerValues = [], backendPlayerValues = [];

  public Task<(bool, TV?)> GetForPlayer<TV>(ulong steam, string statId) {
    if (!cachedPlayerValues.TryGetValue(steam, out var playerStatMap))
      return Task.FromResult<(bool, TV?)>((false, default));
    if (!playerStatMap.TryGetValue(statId, out var value))
      return Task.FromResult<(bool, TV?)>((false, default));
    return value is not TV val ?
      Task.FromResult<(bool, TV?)>((false, default)) :
      Task.FromResult((true, (TV?)val));
  }

  public Task<bool> SetForPlayer<TV>(ulong steam, string statId, TV value) {
    if (!cachedPlayerValues.TryGetValue(steam, out var playerStatMap))
      cachedPlayerValues[steam] =
        playerStatMap = new Dictionary<string, object>();
    if (value == null) return Task.FromResult(false);
    playerStatMap[statId]      = value;
    cachedPlayerValues[steam]  = playerStatMap;
    backendPlayerValues[steam] = playerStatMap;
    return Task.FromResult(true);
  }

  public Task<bool> RemoveFromPlayer(ulong steam, string statId) {
    if (!cachedPlayerValues.TryGetValue(steam, out var playerStatMap))
      return Task.FromResult(false);
    if (!playerStatMap.Remove(statId)) return Task.FromResult(false);
    cachedPlayerValues[steam]  = playerStatMap;
    backendPlayerValues[steam] = playerStatMap;
    return Task.FromResult(true);
  }

  #endregion


  #region Gang

  private readonly Dictionary<int, Dictionary<string, object>>
    cachedGangValues = [], backendGangValues = [];

  public Task<bool> GetForGang<TV>(int key, string statId, out TV? holder) {
    holder = default;
    if (!cachedGangValues.TryGetValue(key, out var gangStatMap))
      return Task.FromResult(false);
    if (!gangStatMap.TryGetValue(statId, out var value))
      return Task.FromResult(false);
    if (value is not TV v) return Task.FromResult(false);
    holder = v;
    return Task.FromResult(true);
  }

  public Task<(bool, TV?)> GetForGang<TV>(int key, string statId) {
    if (!cachedGangValues.TryGetValue(key, out var gangStatMap))
      return Task.FromResult<(bool, TV?)>((false, default));
    if (!gangStatMap.TryGetValue(statId, out var value))
      return Task.FromResult<(bool, TV?)>((false, default));
    return value is not TV val ?
      Task.FromResult<(bool, TV?)>((false, default)) :
      Task.FromResult((true, (TV?)val));
  }

  public Task<bool> SetForGang<TV>(int gangId, string statId, TV value) {
    if (!cachedGangValues.TryGetValue(gangId, out var gangStatMap))
      cachedGangValues[gangId] = gangStatMap = new Dictionary<string, object>();
    if (value == null) return Task.FromResult(false);
    gangStatMap[statId]       = value;
    cachedGangValues[gangId]  = gangStatMap;
    backendGangValues[gangId] = gangStatMap;
    return Task.FromResult(true);
  }

  public Task<bool> RemoveFromGang(int gangId, string statId) {
    if (!cachedGangValues.TryGetValue(gangId, out var gangStatMap))
      return Task.FromResult(false);
    if (!gangStatMap.Remove(statId)) return Task.FromResult(false);
    cachedGangValues[gangId]  = gangStatMap;
    backendGangValues[gangId] = gangStatMap;
    return Task.FromResult(true);
  }

  #endregion
}