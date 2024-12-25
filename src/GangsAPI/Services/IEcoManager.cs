using GangsAPI.Data;
using GangsAPI.Data.Gang;

namespace GangsAPI.Services;

public interface IEcoManager : IPluginBehavior {
  Task<bool> CanAfford(PlayerWrapper player, int cost,
    bool excludeGangCredits = false);

  Task<int> GetBalance(PlayerWrapper player, bool excludeGangCredits = false);

  Task<int> GetBalance(int gangId);
  Task<int> GetBalance(IGang gang) => GetBalance(gang.GangId);

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
  /// <param name="excludeGangCredits"></param>
  /// <returns></returns>
  Task<int> TryPurchase(PlayerWrapper player, int balanceDue, bool print = true,
    string? item = null, bool excludeGangCredits = false);

  Task<int> Grant(PlayerWrapper player, int amount, bool print = true,
    string? reason = null);

  Task<int> Grant(IGang gang, int amount, bool print = true,
    string? reason = null) {
    return Grant(gang.GangId, amount, print, reason);
  }

  Task<int> Grant(int gangId, int amount, bool print = true,
    string? reason = null);
}