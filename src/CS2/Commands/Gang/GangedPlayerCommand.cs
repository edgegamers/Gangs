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
using GangsAPI.Services.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace Commands.Gang;

/// <summary>
///   Represents a command only executable by those in gangs.
/// </summary>
/// <param name="provider"></param>
public abstract class GangedPlayerCommand(IServiceProvider provider)
  : ICommand {
  protected readonly ICommandManager Commands =
    provider.GetRequiredService<ICommandManager>();

  protected readonly IEcoManager Eco =
    provider.GetRequiredService<IEcoManager>();

  protected readonly IGangChatPerk? GangChat =
    provider.GetService<IGangChatPerk>();

  protected readonly IGangManager Gangs =
    provider.GetRequiredService<IGangManager>();

  protected readonly IGangStatManager GangStats =
    provider.GetRequiredService<IGangStatManager>();

  protected readonly IStringLocalizer Locale =
    provider.GetRequiredService<IStringLocalizer>();

  protected readonly IMenuManager Menus =
    provider.GetRequiredService<IMenuManager>();

  protected readonly IPlayerManager Players =
    provider.GetRequiredService<IPlayerManager>();

  protected readonly IPlayerStatManager PlayerStats =
    provider.GetRequiredService<IPlayerStatManager>();

  protected readonly IServiceProvider Provider = provider;

  protected readonly IRankManager Ranks =
    provider.GetRequiredService<IRankManager>();

  protected readonly IGangTargeter GangTargeter =
    provider.GetRequiredService<IGangTargeter>();

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
      info.ReplySync(Locale.Get(MSG.NOT_IN_GANG));
      return CommandResult.SUCCESS;
    }

    return await Execute(executor, gangPlayer, info);
  }

  abstract protected Task<CommandResult> Execute(PlayerWrapper executor,
    IGangPlayer player, CommandInfoWrapper info);
}