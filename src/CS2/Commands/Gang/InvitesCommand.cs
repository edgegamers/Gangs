using Commands.Menus;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Permissions;
using GangsAPI.Services;
using GangsAPI.Services.Commands;
using GangsAPI.Services.Gang;
using GangsAPI.Services.Menu;
using GangsAPI.Services.Player;
using Microsoft.Extensions.Localization;

namespace Commands.Gang;

public class InvitesCommand(IPlayerManager players, IRankManager ranks,
  IMenuManager menus, IGangManager gangs, IGangStatManager gangStats,
  IStringLocalizer locale) : ICommand {
  public string Name => "invites";
  public string[] Usage => [""];

  public async Task<CommandResult> Execute(PlayerWrapper? executor,
    CommandInfoWrapper info) {
    if (executor == null) return CommandResult.PLAYER_ONLY;
    var gangPlayer = await players.GetPlayer(executor.Steam);
    if (gangPlayer == null) {
      info.ReplySync(
        locale.Get(MSG.GENERIC_ERROR_INFO, "Gang Player not found"));
      return CommandResult.ERROR;
    }

    if (gangPlayer.GangId == null || gangPlayer.GangRank == null) {
      info.ReplySync(locale.Get(MSG.NOT_IN_GANG));
      return CommandResult.SUCCESS;
    }

    var (permitted, minimum) =
      await ranks.CheckRank(gangPlayer, Perm.INVITE_OTHERS);

    if (!permitted) {
      info.ReplySync(locale.Get(MSG.GENERIC_NOPERM_RANK,
        minimum?.Name ?? "Unknown"));
      return CommandResult.NO_PERMISSION;
    }

    var gang = await gangs.GetGang(gangPlayer.GangId.Value);

    if (gang == null) {
      info.ReplySync(locale.Get(MSG.GENERIC_ERROR_INFO, "Gang not found"));
      return CommandResult.ERROR;
    }

    var menu = new OutgoingInvitesMenu(menus, gang, gangStats, players);
    await menus.OpenMenu(executor, menu);
    return CommandResult.SUCCESS;
  }
}