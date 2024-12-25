using Commands.Menus;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Services.Commands;
using GangsAPI.Services.Gang;
using GangsAPI.Services.Menu;
using GangsAPI.Services.Player;
using GangsAPI.Services.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace Commands.Gang;

public class MembersCommand(IServiceProvider provider) : ICommand {
  private readonly IGangManager gangs =
    provider.GetRequiredService<IGangManager>();

  private readonly IGangTargeter gangTargeter =
    provider.GetRequiredService<IGangTargeter>();

  private readonly IStringLocalizer locale =
    provider.GetRequiredService<IStringLocalizer>();

  private readonly IMenuManager menus =
    provider.GetRequiredService<IMenuManager>();

  private readonly IPlayerManager players =
    provider.GetRequiredService<IPlayerManager>();

  private readonly IPlayerTargeter playerTargeter =
    provider.GetRequiredService<IPlayerTargeter>();

  public string Name => "members";
  public string[] Usage => ["", "<gang/player>"];
  public string? Description => "View the members of a gang";

  public async Task<CommandResult> Execute(PlayerWrapper? executor,
    CommandInfoWrapper info) {
    if (executor == null) return CommandResult.PLAYER_ONLY;
    if (info.ArgCount > 2) return CommandResult.PRINT_USAGE;
    int gangId;

    if (info.ArgCount == 1) {
      var gangPlayer = await players.GetPlayer(executor.Steam);
      if (gangPlayer == null) {
        info.ReplySync(locale.Get(MSG.GENERIC_ERROR_INFO,
          "Gang Player not found"));
        return CommandResult.ERROR;
      }

      if (gangPlayer.GangId == null || gangPlayer.GangRank == null) {
        info.ReplySync(locale.Get(MSG.NOT_IN_GANG));
        return CommandResult.SUCCESS;
      }

      gangId = gangPlayer.GangId.Value;
      goto foundPlayer;
    }

    var memberPlayer = await playerTargeter.GetSingleTarget(info[1], executor);
    if (memberPlayer != null) {
      var gangPlayer = await players.GetPlayer(memberPlayer.Steam);
      if (gangPlayer is { GangId : not null, GangRank: not null }) {
        gangId = gangPlayer.GangId.Value;
        goto foundPlayer;
      }
    }
    // Regardless of if an online player was found, if we reached this spot,
    // we did not find a valid gang by player. So, look for a gang by name.

    var foundGang =
      await gangTargeter.FindGang(string.Join(' ', info.Args.Skip(1)),
        executor);

    if (foundGang == null) return CommandResult.SUCCESS;
    gangId = foundGang.GangId;

  foundPlayer:
    var gang = await gangs.GetGang(gangId);
    if (gang == null) {
      info.ReplySync(locale.Get(MSG.GENERIC_ERROR_INFO, "Gang not found"));
      return CommandResult.ERROR;
    }

    await menus.OpenMenu(executor, new MembersMenu(provider, gang));
    return CommandResult.SUCCESS;
  }
}