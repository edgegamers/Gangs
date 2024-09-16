using CounterStrikeSharp.API.Core;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Data.Gang;
using GangsAPI.Exceptions;
using GangsAPI.Services.Commands;
using GangsAPI.Services.Player;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace Commands.Gang;

public abstract class GangedPlayerCommand(IServiceProvider provider)
  : ICommand {
  protected readonly IStringLocalizer Localizer =
    provider.GetRequiredService<IStringLocalizer>();

  protected readonly IPlayerManager Players =
    provider.GetRequiredService<IPlayerManager>();

  protected readonly IServiceProvider Provider = provider;

  public virtual void Start(BasePlugin? plugin, bool hotReload) { }
  public abstract string Name { get; }
  public virtual string? Description => null;
  public virtual string[] RequiredFlags => [];
  public virtual string[] RequiredGroups => [];
  public virtual string[] Aliases => [Name];
  public virtual string[] Usage => [];

  public async Task<CommandResult> Execute(PlayerWrapper? executor,
    CommandInfoWrapper info) {
    if (executor == null) return CommandResult.PLAYER_ONLY;

    var gangPlayer = await Players.GetPlayer(executor.Steam)
      ?? throw new PlayerNotFoundException(executor.Steam);
    if (gangPlayer.GangId == null || gangPlayer.GangRank == null) {
      info.ReplySync(Localizer.Get(MSG.NOT_IN_GANG));
      return CommandResult.SUCCESS;
    }

    return await Execute(executor, gangPlayer, info);
  }

  abstract protected Task<CommandResult> Execute(PlayerWrapper executor,
    IGangPlayer player, CommandInfoWrapper info);
}