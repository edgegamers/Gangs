using System.Diagnostics;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Data.Gang;
using GangsAPI.Exceptions;

namespace Commands.Gang;

public class LeaveCommand(IServiceProvider provider)
  : GangedPlayerCommand(provider) {
  public override string Name => "leave";

  override protected async Task<CommandResult> Execute(PlayerWrapper executor,
    IGangPlayer player, CommandInfoWrapper info) {
    Debug.Assert(player.GangId != null, "player.GangId != null");

    var gang = await Gangs.GetGang(player.GangId.Value)
      ?? throw new GangNotFoundException(player);

    Debug.Assert(player.GangRank != null, "player.GangRank != null");
    if (player.GangRank.Value == 0) {
      info.ReplySync(Localizer.Get(MSG.RANK_CANNOT_OWNER, "leave"));
      return CommandResult.SUCCESS;
    }

    if (GangChat != null)
      await GangChat.SendGangChat(player, gang,
        Localizer.Get(MSG.COMMAND_LEAVE_LEFT,
          player.Name ?? player.Steam.ToString()));

    player.GangId   = null;
    player.GangRank = null;

    await Players.UpdatePlayer(player);
    return CommandResult.SUCCESS;
  }
}