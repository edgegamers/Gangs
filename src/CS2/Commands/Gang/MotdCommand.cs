using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Command;
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

public class MotdCommand(IServiceProvider provider) : ICommand {
  public string Name => "motd";

  public string[] Aliases => ["motd", "description"];

  public string[] Usage => ["[motd]"];

  private readonly IPlayerManager players =
    provider.GetRequiredService<IPlayerManager>();

  private readonly IStringLocalizer localizer =
    provider.GetRequiredService<IStringLocalizer>();

  private readonly IRankManager ranks =
    provider.GetRequiredService<IRankManager>();

  private readonly IGangManager gangs =
    provider.GetRequiredService<IGangManager>();

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

    var (success, required) =
      await ranks.CheckRank(gangPlayer, Perm.MANAGE_PERKS);

    if (required == null)
      throw new SufficientRankNotFoundException(gangPlayer.GangId.Value,
        Perm.OWNER);

    if (!success) {
      info.ReplySync(localizer.Get(MSG.GENERIC_NOPERM_RANK, required.Name));
      return CommandResult.SUCCESS;
    }

    if (rank != 0)
      throw new GangException("Passed rank check but not numerical check");

    if (info.ArgCount == 1) { return CommandResult.PRINT_USAGE; }

    var motdManager = provider.GetService<IMotdPerk>();

    if (motdManager == null) {
      info.ReplySync(localizer.Get(MSG.GENERIC_ERROR,
        "MOTD perk is not enabled"));
      return CommandResult.ERROR;
    }

    var gang = await gangs.GetGang(gangPlayer.GangId.Value)
      ?? throw new GangNotFoundException(gangPlayer.GangId.Value);

    var motd = string.Join(" ", info.Args.Skip(1));

    await motdManager.SetMotd(gangPlayer.GangId.Value, motd);

    info.ReplySync(localizer.Get(MSG.PERK_MOTD_SET, gang.Name, motd));
    return CommandResult.SUCCESS;
  }
}