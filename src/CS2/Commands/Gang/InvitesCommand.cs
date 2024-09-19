using System.Diagnostics;
using Commands.Menus;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Data.Gang;
using GangsAPI.Exceptions;
using GangsAPI.Permissions;

namespace Commands.Gang;

public class InvitesCommand(IServiceProvider provider)
  : GangedPlayerCommand(provider) {
  public override string Name => "invites";
  public override string[] Usage => [""];

  public override async Task<CommandResult> Execute(PlayerWrapper? executor,
    CommandInfoWrapper info) {
    if (executor == null) return CommandResult.PLAYER_ONLY;
    var gangPlayer = await Players.GetPlayer(executor.Steam)
      ?? throw new PlayerNotFoundException(executor.Steam);

    if (gangPlayer.GangId == null || gangPlayer.GangRank == null) {
      await Commands.ProcessCommand(executor, info.CallingContext, "css_gang",
        "pendinginvites");
      return CommandResult.SUCCESS;
    }

    return await Execute(executor, gangPlayer, info);
  }

  override protected async Task<CommandResult> Execute(PlayerWrapper executor,
    IGangPlayer player, CommandInfoWrapper info) {
    var (permitted, minimum) =
      await Ranks.CheckRank(player, Perm.INVITE_OTHERS);

    if (!permitted) {
      info.ReplySync(Localizer.Get(MSG.GENERIC_NOPERM_RANK, minimum.Name));
      return CommandResult.NO_PERMISSION;
    }

    Debug.Assert(player.GangId != null, "player.GangId != null");
    var gang = await Gangs.GetGang(player.GangId.Value)
      ?? throw new GangNotFoundException(player.GangId.Value);

    var menu = new OutgoingInvitesMenu(Provider, gang);
    await Menus.OpenMenu(executor, menu);
    return CommandResult.SUCCESS;
  }
}