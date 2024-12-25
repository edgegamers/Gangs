using System.Diagnostics;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Data.Gang;
using GangsAPI.Exceptions;
using Stats.Stat;

namespace Commands.Gang;

/// <summary>
/// Reports the balance of the gang, if any.
/// </summary>
/// <param name="provider"></param>
public class BalanceCommand(IServiceProvider provider)
  : GangedPlayerCommand(provider) {
  public override string Name => "balance";
  public override string[] Aliases => [Name, "credits", "bank"];
  public override string[] Usage => ["", "<gang>"];
  public override string? Description => "Check the balance of a gang";

  override protected async Task<CommandResult> Execute(PlayerWrapper executor,
    IGangPlayer player, CommandInfoWrapper info) {
    Debug.Assert(player.GangId != null, "player.GangId != null");

    var gangId = player.GangId.Value;

    if (info.ArgCount > 1) {
      var gang = await GangTargeter.FindGang(info.Args[1], executor);
      if (gang == null) return CommandResult.ERROR;

      gangId = gang.GangId;
    }

    await reportBalance(info, gangId);
    return CommandResult.SUCCESS;
  }

  private async Task reportBalance(CommandInfoWrapper info, int gangId) {
    var (success, balance) =
      await GangStats.GetForGang<int>(gangId, BalanceStat.STAT_ID);

    var gang = await Gangs.GetGang(gangId)
      ?? throw new GangNotFoundException(gangId);

    if (!success) {
      info.ReplySync(Locale.Get(MSG.COMMAND_BALANCE_GANG_NONE, gang.Name));
      return;
    }

    info.ReplySync(Locale.Get(MSG.COMMAND_BALANCE_GANG, gang.Name, balance));
  }
}