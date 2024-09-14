using CounterStrikeSharp.API.Core;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Data.Gang;
using GangsAPI.Exceptions;
using GangsAPI.Perks;
using GangsAPI.Permissions;
using GangsAPI.Services;
using GangsAPI.Services.Commands;
using GangsAPI.Services.Gang;
using GangsAPI.Services.Player;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace Commands.Gang;

public class DisbandCommand(IServiceProvider provider) : ICommand {
  public string Name => "disband";

  private readonly IPlayerManager players =
    provider.GetRequiredService<IPlayerManager>();

  private readonly IStringLocalizer localizer =
    provider.GetRequiredService<IStringLocalizer>();

  private readonly IGangManager gangs =
    provider.GetRequiredService<IGangManager>();

  private readonly IRankManager ranks =
    provider.GetRequiredService<IRankManager>();

  public string[] Usage => ["", "confirm"];

  public async Task<CommandResult> Execute(PlayerWrapper? executor,
    CommandInfoWrapper info) {
    if (executor == null) return CommandResult.PLAYER_ONLY;

    var gangPlayer = await players.GetPlayer(executor.Steam)
      ?? throw new PlayerNotFoundException(executor.Steam);

    if (gangPlayer.GangId == null) {
      info.ReplySync(localizer.Get(MSG.NOT_IN_GANG));
      return CommandResult.SUCCESS;
    }

    var rank = gangPlayer.GangRank;

    var (success, required) = await ranks.CheckRank(gangPlayer, Perm.OWNER);
    if (required == null)
      throw new SufficientRankNotFoundException(gangPlayer.GangId.Value,
        Perm.OWNER);

    if (!success) {
      info.ReplySync(localizer.Get(MSG.GENERIC_NOPERM_RANK, required.Name));
      return CommandResult.SUCCESS;
    }

    if (rank != 0)
      throw new GangException("Passed rank check but not numerical check");

    if (info.ArgCount == 1) {
      info.ReplySync(localizer.Get(MSG.COMMAND_GANG_DISBAND_WARN));
      return CommandResult.SUCCESS;
    }

    var gang = await gangs.GetGang(gangPlayer.GangId.Value)
      ?? throw new GangNotFoundException(gangPlayer.GangId.Value);

    if (info[1] != "confirm") return CommandResult.PRINT_USAGE;

    var gangChat = provider.GetService<IGangChatPerk>();
    if (gangChat != null) {
      await gangChat.SendGangChat(gang,
        localizer.Get(MSG.COMMAND_GANG_DISBANDED,
          executor.Name ?? executor.Steam.ToString()));
    }

    await gangs.DeleteGang(gangPlayer.GangId.Value);
    return CommandResult.SUCCESS;
  }
}