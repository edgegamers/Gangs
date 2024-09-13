using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Gang;
using GangsAPI.Permissions;
using GangsAPI.Services;
using GangsAPI.Services.Gang;
using GangsAPI.Services.Player;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Stats.Stat;

namespace GangsImpl;

public class EcoManager(IServiceProvider provider) : IEcoManager {
  private readonly IGangStatManager gangStats =
    provider.GetRequiredService<IGangStatManager>();

  private readonly IStringLocalizer localizer =
    provider.GetRequiredService<IStringLocalizer>();

  private readonly IPlayerStatManager playerStats =
    provider.GetRequiredService<IPlayerStatManager>();

  private readonly IRankManager ranks =
    provider.GetRequiredService<IRankManager>();

  private readonly string statId = new BalanceStat().StatId;

  public async Task<bool> CanAfford(IGangPlayer player, int cost) {
    return (await getBalance(player)).Item2 >= cost;
  }

  public async Task<int> TryPurchase(IGangPlayer player, int cost,
    bool print = true, string? item = null) {
    var wrapper = new PlayerWrapper(player);
    var (playerBalance, totalBalance) = await getBalance(player);
    var gangBalance = totalBalance - playerBalance;
    var remaining   = totalBalance - cost;

    bool usedBank = false, usedPlayer = false;
    if (remaining < 0) {
      if (!print) return totalBalance;
      if (item == null)
        wrapper.PrintToChat(localizer.Get(MSG.ECO_INSUFFICIENT_FUNDS,
          Math.Abs(remaining)));
      else
        wrapper.PrintToChat(localizer.Get(MSG.ECO_INSUFFICIENT_FUNDS_WITH_ITEM,
          Math.Abs(remaining), item));

      return totalBalance;
    }

    // Pull from gang bank first
    if (player.GangId != null && gangBalance > 0) {
      usedBank = true;

      if (gangBalance >= cost) {
        // Gang can entirely afford the cost
        await gangStats.SetForGang(player.GangId.Value, statId,
          gangBalance - cost);
        remaining -= cost;
        cost      =  0;
      } else {
        // Gang can't entirely afford the cost
        await gangStats.SetForGang(player.GangId.Value, statId,
          0); // Wipe out gang balance
        remaining  -= cost;
        cost       -= gangBalance;
        usedPlayer =  true;
      }
    }

    // Pull remaining from player
    await playerStats.SetForPlayer(player.Steam, statId, playerBalance - cost);
    remaining -= cost;

    if (!print) return remaining;

    if (item == null) {
      switch (usedBank) {
        case true when usedPlayer:
          wrapper.PrintToChat(localizer.Get(MSG.ECO_PURCHASED_WITHBOTH,
            remaining));
          break;
        case true:
          wrapper.PrintToChat(localizer.Get(MSG.ECO_PURCHASED_WITHGANG,
            gangBalance - cost));
          return gangBalance - cost;
        default: {
          if (usedPlayer)
            wrapper.PrintToChat(localizer.Get(MSG.ECO_PURCHASED, remaining));
          break;
        }
      }

      return remaining;
    }

    switch (usedBank) {
      case true when usedPlayer:
        wrapper.PrintToChat(localizer.Get(MSG.ECO_PURCHASED_WITHBOTH_ITEM, item,
          remaining));
        break;
      case true:
        wrapper.PrintToChat(localizer.Get(MSG.ECO_PURCHASED_WITHGANG_ITEM, item,
          gangBalance - cost));
        return gangBalance - cost;
      default: {
        if (usedPlayer)
          wrapper.PrintToChat(localizer.Get(MSG.ECO_PURCHASED_WITHITEM, item,
            remaining));
        break;
      }
    }

    return remaining;
  }

  public Task<int> Grant(ulong player, int amount, string reason = "") {
    throw new NotImplementedException();
  }

  public Task<int> Grant(int gangId, int amount, string reason = "") {
    throw new NotImplementedException();
  }

  private async Task<(int, int)> getBalance(IGangPlayer player) {
    var total = 0;
    var (success, balance) =
      await playerStats.GetForPlayer<int>(player.Steam, statId);
    if (success) total += balance;
    var playerTotal    = total;

    if (player.GangId == null || player.GangRank == null)
      return (playerTotal, total);
    var rank = await ranks.GetRank(player);
    if (rank == null) return (playerTotal, total);
    if (!rank.Permissions.HasFlag(Perm.BANK_WITHDRAW))
      return (playerTotal, total);
    var (gSuccess, gBalance) =
      await gangStats.GetForGang<int>(player.GangId.Value, statId);
    if (gSuccess) total += gBalance;

    return (playerTotal, total);
  }
}