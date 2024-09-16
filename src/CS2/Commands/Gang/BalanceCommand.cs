using System.Diagnostics;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Data.Gang;
using GangsAPI.Exceptions;
using Stats.Stat;

namespace Commands.Gang;

public class BalanceCommand(IServiceProvider provider)
  : GangedPlayerCommand(provider) {
  private readonly string id = new BalanceStat().StatId;

  public override string Name => "balance";

  override protected async Task<CommandResult> Execute(PlayerWrapper executor,
    IGangPlayer player, CommandInfoWrapper info) {
    Debug.Assert(player.GangId != null, "player.GangId != null");
    var (success, balance) =
      await GangStats.GetForGang<int>(player.GangId.Value, id);

    var gang = await Gangs.GetGang(player.GangId.Value)
      ?? throw new GangNotFoundException(player.GangId.Value);

    if (!success) {
      info.ReplySync(Localizer.Get(MSG.COMMAND_BALANCE_GANG_NONE, gang.Name));
      return CommandResult.SUCCESS;
    }

    info.ReplySync(Localizer.Get(MSG.COMMAND_BALANCE_GANG, gang.Name, balance));
    return CommandResult.SUCCESS;
  }
}