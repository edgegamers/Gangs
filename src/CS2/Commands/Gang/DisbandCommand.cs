using System.Diagnostics;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Data.Gang;
using GangsAPI.Exceptions;
using GangsAPI.Permissions;

namespace Commands.Gang;

public class DisbandCommand(IServiceProvider provider)
  : GangedPlayerCommand(provider) {
  public override string Name => "disband";
  public override string[] Usage => ["", "confirm"];

  override protected async Task<CommandResult> Execute(PlayerWrapper executor,
    IGangPlayer player, CommandInfoWrapper info) {
    var rank = player.GangRank;

    Debug.Assert(player.GangId != null, "player.GangId != null");
    var (success, required) = await Ranks.CheckRank(player, Perm.OWNER);
    if (required == null) {
      throw new SufficientRankNotFoundException(player.GangId.Value,
        Perm.OWNER);
    }

    if (!success) {
      info.ReplySync(Localizer.Get(MSG.GENERIC_NOPERM_RANK, required.Name));
      return CommandResult.SUCCESS;
    }

    if (rank != 0)
      throw new GangException("Passed rank check but not numerical check");

    if (info.ArgCount == 1) {
      info.ReplySync(Localizer.Get(MSG.COMMAND_GANG_DISBAND_WARN));
      return CommandResult.SUCCESS;
    }

    var gang = await Gangs.GetGang(player.GangId.Value)
      ?? throw new GangNotFoundException(player.GangId.Value);

    if (info[1] != "confirm") return CommandResult.PRINT_USAGE;

    if (GangChat != null)
      await GangChat.SendGangChat(gang,
        Localizer.Get(MSG.COMMAND_GANG_DISBANDED,
          executor.Name ?? executor.Steam.ToString()));

    await Gangs.DeleteGang(player.GangId.Value);
    return CommandResult.SUCCESS;
  }
}