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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Stats.Stat;

namespace Commands.Gang;

public class InvitesCommand(IServiceProvider provider) : ICommand {
  private readonly IGangManager gangs =
    provider.GetRequiredService<IGangManager>();

  private readonly IGangStatManager gangStats =
    provider.GetRequiredService<IGangStatManager>();

  private readonly IStringLocalizer locale =
    provider.GetRequiredService<IStringLocalizer>();

  private readonly IMenuManager menus =
    provider.GetRequiredService<IMenuManager>();

  private readonly IPlayerManager players =
    provider.GetRequiredService<IPlayerManager>();

  private readonly IRankManager ranks =
    provider.GetRequiredService<IRankManager>();

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

    var (success, gangInvites) =
      await gangStats.GetForGang<InvitationData>(gang,
        new InvitationStat().StatId);

    if (!success || gangInvites == null
      || gangInvites.GetInvitedSteams().Count == 0) {
      info.ReplySync(locale.Get(MSG.COMMAND_INVITE_NONE));
      return CommandResult.SUCCESS;
    }

    var menu = new OutgoingInvitesMenu(provider, gang);
    await menus.OpenMenu(executor, menu);
    return CommandResult.SUCCESS;
  }
}