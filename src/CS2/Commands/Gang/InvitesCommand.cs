using System.Diagnostics;
using Commands.Menus;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Data.Gang;
using GangsAPI.Exceptions;
using GangsAPI.Permissions;
using GangsAPI.Services;
using GangsAPI.Services.Commands;
using GangsAPI.Services.Gang;
using GangsAPI.Services.Menu;
using GangsAPI.Services.Player;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace Commands.Gang;

/// <summary>
/// TODO: Dont make this a GangedPlayerCommand
/// </summary>
/// <param name="provider"></param>
public class InvitesCommand(IServiceProvider provider) : ICommand {
  private readonly IPlayerManager players =
    provider.GetRequiredService<IPlayerManager>();

  private readonly IPlayerManager Players =
    provider.GetRequiredService<IPlayerManager>();

  private readonly IRankManager Ranks =
    provider.GetRequiredService<IRankManager>();

  private readonly IGangManager Gangs =
    provider.GetRequiredService<IGangManager>();

  private readonly IMenuManager Menus =
    provider.GetRequiredService<IMenuManager>();

  private readonly IStringLocalizer Locale =
    provider.GetRequiredService<IStringLocalizer>();

  private readonly ICommandManager Commands =
    provider.GetRequiredService<ICommandManager>();


  public string Name => "invites";
  public string[] Usage => ["", "<player/steam>"];
  public string Description => "View and manage incoming invites";

  public async Task<CommandResult> Execute(PlayerWrapper? executor,
    CommandInfoWrapper info) {
    if (executor == null) return CommandResult.PLAYER_ONLY;
    var gangPlayer = await Players.GetPlayer(executor.Steam)
      ?? throw new PlayerNotFoundException(executor.Steam);

    return await Execute(executor, gangPlayer, info);
  }

  protected async Task<CommandResult> Execute(PlayerWrapper executor,
    IGangPlayer player, CommandInfoWrapper info) {
    var (permitted, minimum) =
      await Ranks.CheckRank(player, Perm.INVITE_OTHERS);

    if (!permitted) {
      info.ReplySync(Locale.Get(MSG.GENERIC_NOPERM_RANK, minimum.Name));
      return CommandResult.NO_PERMISSION;
    }

    Debug.Assert(player.GangId != null, "player.GangId != null");
    var gang = await Gangs.GetGang(player.GangId.Value)
      ?? throw new GangNotFoundException(player.GangId.Value);

    if (info.ArgCount == 2) {
      var gangPlayer = await searchPlayer(gang, info.Args[1]);
      if (gangPlayer == null) {
        info.ReplySync(Locale.Get(MSG.GENERIC_PLAYER_NOT_FOUND, info.Args[1]));
        return CommandResult.SUCCESS;
      }

      var menu = new OutgoingInviteMenu(provider, gang, gangPlayer.Steam);
      await Menus.OpenMenu(executor, menu);
    } else {
      var menu = new OutgoingInvitesMenu(provider, gang);
      await Menus.OpenMenu(executor, menu);
    }

    return CommandResult.SUCCESS;
  }

  private async Task<IGangPlayer?> searchPlayer(IGang gang, string query) {
    var members = (await players.GetMembers(gang.GangId)).ToList();
    var player = members.FirstOrDefault(p
      => query.Equals(p.Name, StringComparison.OrdinalIgnoreCase));
    if (player != null) return player;

    if (!ulong.TryParse(query, out var id)) return null;
    return members.FirstOrDefault(p => p.Steam == id);
  }
}