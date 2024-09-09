using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Commands.Targeting;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Services.Commands;
using GangsAPI.Services.Player;
using Microsoft.Extensions.Localization;
using Stats;

namespace Commands;

public class BalanceCommand(IPlayerStatManager playerMgr,
  IStringLocalizer localizer) : ICommand {
  private readonly string id = new BalanceStat().StatId;
  public string Name => "css_balance";

  public string[] Usage => ["", "<player>", "<player> <amount>"];
  public string[] Aliases => ["css_balance", "css_credit", "css_credits"];

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

    if (info.ArgCount > 3) return CommandResult.PRINT_USAGE;

    // TODO: Add Unit Test Support
    // Would require a mock of some type of Server state
    // for Utilities to wrap around.
    var            result  = CommandResult.ERROR;
    PlayerWrapper? subject = null;
    await Server.NextFrameAsync(() => {
      var target       = new Target(info[1]);
      var targetResult = target.GetTarget(null).Players;
      if (targetResult.Count != 1) {
        info.ReplySync(localizer.Get(
          targetResult.Count > 1 ?
            MSG.GENERIC_PLAYER_FOUND_MULTIPLE :
            MSG.GENERIC_PLAYER_NOT_FOUND, info[1]));
        result = CommandResult.INVALID_ARGS;
        return;
      }

      subject = new PlayerWrapper(targetResult[0]);
    });

    if (subject == null) return result;

    if (info.ArgCount == 2 || !executor.HasFlags("@css/root")) {
      var (success, balance) =
        await playerMgr.GetForPlayer<int>(subject.Steam, id);

      if (!success) {
        info.ReplySync(localizer.Get(MSG.COMMAND_BALANCE_OTHER_NONE,
          subject.Name ?? subject.Steam.ToString()));
        return CommandResult.SUCCESS;
      }

      info.ReplySync(localizer.Get(
        balance == 1 ?
          MSG.COMMAND_BALANCE_OTHER :
          MSG.COMMAND_BALANCE_OTHER_PLURAL,
        subject.Name ?? subject.Steam.ToString(), balance));
      return CommandResult.SUCCESS;
    }

    if (!int.TryParse(info[2], out var amount)) {
      info.ReplySync(localizer.Get(MSG.COMMAND_INVALID_PARAM, info[2],
        "an integer"));
      return CommandResult.INVALID_ARGS;
    }

    var pass = await playerMgr.SetForPlayer(subject.Steam, id, amount);
    if (!pass) {
      info.ReplySync(localizer.Get(MSG.GENERIC_ERROR));
      return CommandResult.ERROR;
    }

    info.ReplySync(localizer.Get(MSG.COMMAND_BALANCE_SET,
      subject.Name ?? subject.Steam.ToString(), amount));
    return CommandResult.SUCCESS;
  }
}