using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Exceptions;
using GangsAPI.Services.Commands;
using GangsAPI.Services.Gang;
using GangsAPI.Services.Player;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Stats.Stat;

namespace Commands.Gang;

public class BalanceCommand(IServiceProvider provider) : ICommand {
  private readonly IGangManager gangs =
    provider.GetRequiredService<IGangManager>();

  private readonly IGangStatManager gangStats =
    provider.GetRequiredService<IGangStatManager>();

  private readonly string id = new BalanceStat().StatId;

  private readonly IStringLocalizer localizer =
    provider.GetRequiredService<IStringLocalizer>();

  private readonly IPlayerManager players =
    provider.GetRequiredService<IPlayerManager>();

  public string Name => "balance";

  public async Task<CommandResult> Execute(PlayerWrapper? executor,
    CommandInfoWrapper info) {
    if (executor == null) return CommandResult.PLAYER_ONLY;
    if (info.ArgCount != 1) return CommandResult.PRINT_USAGE;
    var gangPlayer = await players.GetPlayer(executor.Steam)
      ?? throw new PlayerNotFoundException(executor.Steam);
    if (gangPlayer.GangId == null) {
      info.ReplySync(localizer.Get(MSG.NOT_IN_GANG));
      return CommandResult.SUCCESS;
    }

    var (success, balance) =
      await gangStats.GetForGang<int>(gangPlayer.GangId.Value, id);

    var gang = await gangs.GetGang(gangPlayer.GangId.Value)
      ?? throw new GangNotFoundException(gangPlayer.GangId.Value);

    if (!success) {
      info.ReplySync(localizer.Get(MSG.COMMAND_BALANCE_GANG_NONE, gang.Name));
      return CommandResult.SUCCESS;
    }

    info.ReplySync(localizer.Get(MSG.COMMAND_BALANCE_GANG, gang.Name, balance));
    return CommandResult.SUCCESS;
  }
}