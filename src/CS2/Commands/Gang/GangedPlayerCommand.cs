using CounterStrikeSharp.API.Core;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Data.Gang;
using GangsAPI.Exceptions;
using GangsAPI.Perks;
using GangsAPI.Services;
using GangsAPI.Services.Commands;
using GangsAPI.Services.Gang;
using GangsAPI.Services.Menu;
using GangsAPI.Services.Player;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace Commands.Gang;

/// <summary>
/// Represents a command only executable by those in gangs.
/// </summary>
/// <param name="provider"></param>
public abstract class GangedPlayerCommand(IServiceProvider provider)
  : ICommand {
  protected readonly IStringLocalizer Localizer =
    provider.GetRequiredService<IStringLocalizer>();

  protected readonly IPlayerManager Players =
    provider.GetRequiredService<IPlayerManager>();

  protected readonly IServiceProvider Provider = provider;

  protected readonly IGangStatManager GangStats =
    provider.GetRequiredService<IGangStatManager>();

  protected readonly IPlayerStatManager PlayerStats =
    provider.GetRequiredService<IPlayerStatManager>();

  protected readonly IGangManager Gangs =
    provider.GetRequiredService<IGangManager>();

  protected readonly IRankManager Ranks =
    provider.GetRequiredService<IRankManager>();

  protected readonly IEcoManager Eco =
    provider.GetRequiredService<IEcoManager>();

  protected readonly IGangChatPerk? GangChat =
    provider.GetService<IGangChatPerk>();

  protected readonly ICommandManager Commands =
    provider.GetRequiredService<ICommandManager>();

  protected readonly IMenuManager Menus =
    provider.GetRequiredService<IMenuManager>();

  public virtual void Start(BasePlugin? plugin, bool hotReload) { }
  public abstract string Name { get; }
  public virtual string? Description => null;
  public virtual string[] RequiredFlags => [];
  public virtual string[] RequiredGroups => [];
  public virtual string[] Aliases => [Name];
  public virtual string[] Usage => [];

  public virtual async Task<CommandResult> Execute(PlayerWrapper? executor,
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