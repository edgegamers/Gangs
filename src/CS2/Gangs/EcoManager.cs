using System.Diagnostics;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Gang;
using GangsAPI.Exceptions;
using GangsAPI.Perks;
using GangsAPI.Permissions;
using GangsAPI.Services;
using GangsAPI.Services.Gang;
using GangsAPI.Services.Player;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Stats.Stat;

namespace GangsImpl;

public class EcoManager(IServiceProvider provider) : IEcoManager {
  private readonly IPlayerManager players =
    provider.GetRequiredService<IPlayerManager>();

  private readonly IGangManager gangs =
    provider.GetRequiredService<IGangManager>();

  private readonly IGangStatManager gangStats =
    provider.GetRequiredService<IGangStatManager>();

  private readonly IStringLocalizer localizer =
    provider.GetRequiredService<IStringLocalizer>();

  private readonly IPlayerStatManager playerStats =
    provider.GetRequiredService<IPlayerStatManager>();

  private readonly IRankManager ranks =
    provider.GetRequiredService<IRankManager>();

  private readonly IGangChatPerk? gangChat =
    provider.GetService<IGangChatPerk>();

  private readonly string statId = new BalanceStat().StatId;

  public async Task<bool> CanAfford(PlayerWrapper player, int cost,
    bool excludeGangCredits = false) {
    var (total, playerBalance) = await getBalance(player.Steam);
    return (excludeGangCredits ? playerBalance : total) >= cost;
  }

  public async Task<int> TryPurchase(PlayerWrapper player, int balanceDue,
    bool print = true, string? item = null, bool excludeGangCredits = false) {
    var (playerBalance, totalBalance) = await getBalance(player.Steam);
    var gangBalance      = totalBalance - playerBalance;
    var balanceRemaining = totalBalance - balanceDue;

    bool usedBank = false, usedPlayer = false;
    if (balanceRemaining < 0) {
      // Can't afford
      if (!print) return totalBalance;
      if (item == null)
        player.PrintToChat(localizer.Get(MSG.ECO_INSUFFICIENT_FUNDS,
          Math.Abs(balanceRemaining)));
      else
        player.PrintToChat(localizer.Get(MSG.ECO_INSUFFICIENT_FUNDS_WITH_ITEM,
          Math.Abs(balanceRemaining), item));

      return totalBalance;
    }

    var gangPlayer = await players.GetPlayer(player.Steam)
      ?? throw new PlayerNotFoundException(player.Steam);

    // Pull from gang bank first
    if (!excludeGangCredits && gangPlayer.GangId != null && gangBalance > 0) {
      usedBank = true;
      var deductFromGang = Math.Min(gangBalance, balanceDue);
      await Grant(gangPlayer.GangId.Value, -deductFromGang, print, item);

      if (gangBalance < balanceDue) usedPlayer = true;

      balanceDue       -= deductFromGang;
      balanceRemaining -= deductFromGang;
    }

    Debug.Assert(balanceDue >= 0,
      $"Expected balanceDue to be >= 0, got {balanceDue}");

    // Pull remaining from player
    if (balanceDue > 0) {
      usedPlayer = true;
      await Grant(player.Steam, -balanceDue, print, item);
    }

    balanceRemaining -= balanceDue;

    if (!print) return balanceRemaining;

    var playerBankMsg = localizer.Get(MSG.ECO_PLAYER_GIVE_NEGATIVE, balanceDue,
      item ?? "Unknown");

    player.PrintToChat(playerBankMsg);

    if (item == null) {
      switch (usedBank) {
        case true when usedPlayer:
          player.PrintToChat(localizer.Get(MSG.ECO_PURCHASED_WITHBOTH,
            balanceRemaining));
          break;
        case true:
          player.PrintToChat(localizer.Get(MSG.ECO_PURCHASED_WITHGANG,
            gangBalance - balanceDue));
          return gangBalance - balanceDue;
        default: {
          if (usedPlayer)
            player.PrintToChat(localizer.Get(MSG.ECO_PURCHASED,
              balanceRemaining));
          break;
        }
      }

      return balanceRemaining;
    }

    switch (usedBank) {
      case true when usedPlayer:
        player.PrintToChat(localizer.Get(MSG.ECO_PURCHASED_WITHBOTH_ITEM, item,
          balanceRemaining));
        break;
      case true:
        player.PrintToChat(localizer.Get(MSG.ECO_PURCHASED_WITHGANG_ITEM, item,
          gangBalance - balanceDue));
        return gangBalance - balanceDue;
      default: {
        if (usedPlayer)
          player.PrintToChat(localizer.Get(MSG.ECO_PURCHASED_WITHITEM, item,
            balanceRemaining));
        break;
      }
    }

    return balanceRemaining;
  }

  public async Task<int> Grant(ulong steam, int amount, bool print = true,
    string? reason = null) {
    var player = await players.GetPlayer(steam)
      ?? throw new PlayerNotFoundException(steam);
    var wrapper = new PlayerWrapper(player);
    var (_, playerBalance) = await getBalance(steam);

    await playerStats.SetForPlayer(player.Steam, statId,
      playerBalance + amount);

    if (!print) return playerBalance + amount;

    var msg = amount < 0 ?
      MSG.ECO_PLAYER_GIVE_NEGATIVE :
      MSG.ECO_PLAYER_GIVE_POSITIVE;

    wrapper.PrintToChat(localizer.Get(msg, amount, reason ?? "Unknown"));
    return playerBalance + amount;
  }

  public async Task<int> Grant(int gangId, int amount, bool print,
    string? reason = null) {
    var (success, balance) = await gangStats.GetForGang<int>(gangId, statId);
    if (!success) balance = 0;

    await gangStats.SetForGang(gangId, statId, balance + amount);
    if (!print) return balance + amount;

    var gang = await gangs.GetGang(gangId)
      ?? throw new GangNotFoundException(gangId);

    var msg = amount < 0 ?
      MSG.ECO_GANG_GIVE_NEGATIVE :
      MSG.ECO_GANG_GIVE_POSITIVE;

    var gangBankMsg = localizer.Get(msg, amount, reason ?? "Unknown");
    if (!print) return balance + amount;
    if (gangChat != null)
      await gangChat.SendGangChat("SHOP", gang, gangBankMsg);

    return balance + amount;
  }

  private async Task<(int, int)> getBalance(ulong steam) {
    var total = 0;
    var (success, balance) = await playerStats.GetForPlayer<int>(steam, statId);
    if (success) total += balance;
    var playerTotal    = total;

    var gangPlayer = await players.GetPlayer(steam)
      ?? throw new PlayerNotFoundException(steam);

    if (gangPlayer.GangId == null || gangPlayer.GangRank == null)
      return (playerTotal, total);
    var rank = await ranks.GetRank(gangPlayer);
    if (rank == null) return (playerTotal, total);
    if (!rank.Permissions.HasFlag(Perm.BANK_WITHDRAW))
      return (playerTotal, total);
    var (gSuccess, gBalance) =
      await gangStats.GetForGang<int>(gangPlayer.GangId.Value, statId);
    if (gSuccess) total += gBalance;

    return (playerTotal, total);
  }
}