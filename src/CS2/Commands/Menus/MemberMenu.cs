using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using GangsAPI.Data;
using GangsAPI.Data.Gang;
using GangsAPI.Exceptions;
using GangsAPI.Extensions;
using GangsAPI.Menu;
using GangsAPI.Permissions;
using GangsAPI.Services;
using GangsAPI.Services.Commands;
using GangsAPI.Services.Menu;
using GangsAPI.Services.Player;
using Microsoft.Extensions.DependencyInjection;
using Stats.Stat.Player;

namespace Commands.Menus;

public class MemberMenu(IServiceProvider provider, IGangPlayer member)
  : AbstractMenu<string>(provider.GetRequiredService<IMenuManager>(),
    NativeSenders.Chat) {
  private readonly ICommandManager commands =
    provider.GetRequiredService<ICommandManager>();

  private readonly IPlayerManager players =
    provider.GetRequiredService<IPlayerManager>();

  private readonly IPlayerStatManager playerStats =
    provider.GetRequiredService<IPlayerStatManager>();

  private readonly string playtimeId = new PlaytimeStat().StatId;

  private readonly IRankManager ranks =
    provider.GetRequiredService<IRankManager>();

  override protected async Task<List<string>> GetItems(PlayerWrapper player) {
    var gangPlayer = await players.GetPlayer(player.Steam)
      ?? throw new PlayerNotFoundException(player.Steam);
    var memberRank = await ranks.GetRank(member)
      ?? throw new GangException("Rank not found");
    var viewerRank = await ranks.GetRank(gangPlayer)
      ?? throw new GangException("Rank not found");

    var result = new List<string> {
      $" {ChatColors.LightBlue}{memberRank.Name} {ChatColors.Green}{member.Name} {ChatColors.DarkRed}[{ChatColors.Red}{member.Steam}{ChatColors.DarkRed}]"
    };

    if (viewerRank.Permissions.HasFlag(Perm.PROMOTE_OTHERS))
      result.Add($"{ChatColors.Green}Promote");
    if (viewerRank.Permissions.HasFlag(Perm.DEMOTE_OTHERS))
      result.Add($"{ChatColors.Red}Demote");
    if (viewerRank.Permissions.HasFlag(Perm.KICK_OTHERS))
      result.Add($"{ChatColors.Orange}Kick");

    var (success, data) =
      await playerStats.GetForPlayer<PlaytimeData>(member.Steam, playtimeId);

    if (!success || data == null) return result;

    result.Add(
      $" {ChatColors.Grey}Last Played: {ChatColors.Default}{data.GetLastPlayed().FormatRelative()}");
    return result;
  }

  override protected Task HandleItemSelection(PlayerWrapper player,
    List<string> items, int selectedIndex) {
    var item = items[selectedIndex];

    if (item.Contains("Promote"))
      commands.ProcessCommand(player, CommandCallingContext.Chat, "css_gang",
        "promote", member.Steam.ToString());
    else if (item.Contains("Demote"))
      commands.ProcessCommand(player, CommandCallingContext.Chat, "css_gang",
        "demote", member.Steam.ToString());
    else if (item.Contains("Kick"))
      commands.ProcessCommand(player, CommandCallingContext.Chat, "css_gang",
        "kick", member.Steam.ToString());
    Menus.CloseMenu(player);
    return Task.CompletedTask;
  }

  override protected Task<string> FormatItem(PlayerWrapper player, int index,
    string item) {
    if (item.Contains("Last Played")) return Task.FromResult(item);
    return Task.FromResult($"{index}. {item}");
  }
}