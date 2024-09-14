using GangsAPI.Data.Gang;

namespace GangsAPI.Services;

public interface IEcoManager : IPluginBehavior {
  Task<bool> CanAfford(IGangPlayer player, int cost);

  /// <summary>
  ///   Attempts to purchase an item for the player.
  ///   Returns the resulting balance.
  ///   If the player cannot afford the item, (i.e would cause
  ///   a negative balance), this will fail and return that
  ///   hypothetical (negative) balance.
  /// </summary>
  /// <param name="player"></param>
  /// <param name="balanceDue"></param>
  /// <param name="print"></param>
  /// <param name="item"></param>
  /// <returns></returns>
  Task<int> TryPurchase(IGangPlayer player, int balanceDue, bool print = true,
    string? item = null);

  Task<int> Grant(IGangPlayer player, int amount, bool print = true,
    string? reason = null)
    => Grant(player.Steam, amount, print, reason);

  Task<int> Grant(ulong player, int amount, bool print = true,
    string? reason = null);

  Task<int> Grant(IGang gang, int amount, bool print = true,
    string? reason = null)
    => Grant(gang.GangId, amount, print, reason);

  Task<int> Grant(int gangId, int amount, bool print = true,
    string? reason = null);
}