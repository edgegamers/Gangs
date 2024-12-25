using Commands.Menus;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Exceptions;
using GangsAPI.Services.Commands;
using GangsAPI.Services.Menu;
using GangsAPI.Services.Player;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace Commands.Gang;

public class PerksCommand(IServiceProvider provider) : ICommand {
  private readonly IStringLocalizer locale =
    provider.GetRequiredService<IStringLocalizer>();

  private readonly IMenuManager menus =
    provider.GetRequiredService<IMenuManager>();

  private readonly IPlayerManager players =
    provider.GetRequiredService<IPlayerManager>();

  public string Name => "perks";
  public string? Description => "View and manage the perks of your gang";

  public async Task<CommandResult> Execute(PlayerWrapper? executor,
    CommandInfoWrapper info) {
    if (executor == null) return CommandResult.PLAYER_ONLY;
    var gangPlayer = await players.GetPlayer(executor.Steam)
      ?? throw new PlayerNotFoundException(executor.Steam);
    if (gangPlayer.GangId == null || gangPlayer.GangRank == null) {
      info.ReplySync(locale.Get(MSG.NOT_IN_GANG));
      return CommandResult.SUCCESS;
    }

    await menus.OpenMenu(executor, new PerksMenu(provider));
    return CommandResult.SUCCESS;
  }
}