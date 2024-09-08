using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands.Targeting;
using CounterStrikeSharp.API.Modules.Utils;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Services.Commands;
using GangsAPI.Services.Player;
using Microsoft.Extensions.Localization;
using Stats;

namespace Commands;

public class BalanceCommand(IPlayerStatManager playerMgr,
  IStringLocalizer testLocalizer) : ICommand {
  public string Name => "css_balance";

  public string[] Usage => ["", "<player>", "<player> <amount>"];
  public string[] Aliases => ["css_balance", "css_credit", "css_credits"];

  private string id = new BalanceStat().StatId;
  private IStringLocalizer localizer = testLocalizer;

  public void Start(BasePlugin? plugin, bool hotReload) {
    if (plugin != null) localizer = plugin.Localizer;
  }

  public async Task<CommandResult> Execute(PlayerWrapper? executor,
    CommandInfoWrapper info) {
    if (executor == null) return CommandResult.PLAYER_ONLY;

    if (info.ArgCount == 1 || !executor.HasFlags("@css/ban")) {
      var (success, balance) =
        await playerMgr.GetForPlayer<int>(executor.Steam, id);

      if (!success) {
        info.ReplySync(localizer.Get(MSG.COMMAND_BALANCE_NONE));
        return CommandResult.SUCCESS;
      }

      info.ReplySync(localizer.Get(
        balance == 1 ? MSG.COMMAND_BALANCE : MSG.COMMAND_BALANCE_PLURAL,
        balance));
      return CommandResult.SUCCESS;
    }

    // TODO: Add Unit Test Support
    // Would require a mock of some type of Server state
    // for Utilities to wrap around.
    var target = new Target(info[1]);
    var result = target.GetTarget(null).Players;
    if (result.Count != 1) {
      info.ReplySync(localizer.Get(
        result.Count > 1 ?
          MSG.GENERIC_PLAYER_FOUND_MULTIPLE :
          MSG.GENERIC_PLAYER_NOT_FOUND, info[1]));
      return CommandResult.INVALID_ARGS;
    }

    var subject = result[0];

    if (info.ArgCount == 2 || !executor.HasFlags("@css/root")) {
      var (success, balance) =
        await playerMgr.GetForPlayer<int>(subject.SteamID, id);

      if (!success) {
        info.ReplySync(localizer.Get(MSG.COMMAND_BALANCE_NONE));
        return CommandResult.SUCCESS;
      }

      info.ReplySync(localizer.Get(
        balance == 1 ?
          MSG.COMMAND_BALANCE_OTHER :
          MSG.COMMAND_BALANCE_OTHER_PLURAL, balance));
    }


    if (info.ArgCount != 3) return CommandResult.PRINT_USAGE;

    if (!int.TryParse(info[2], out var amount)) {
      info.ReplySync(localizer.Get(MSG.COMMAND_INVALID_PARAM, info[2],
        "an integer"));
      return CommandResult.INVALID_ARGS;
    }

    var pass = await playerMgr.SetForPlayer(subject.SteamID, id, amount);
    if (!pass) {
      info.ReplySync(localizer.Get(MSG.GENERIC_ERROR));
      return CommandResult.ERROR;
    }

    info.ReplySync(localizer.Get(MSG.COMMAND_BALANCE_SET, subject.PlayerName,
      amount));
    return CommandResult.SUCCESS;
  }
}