using CounterStrikeSharp.API.Modules.Utils;
using GangsAPI.Data;
using GangsAPI.Data.Gang;
using GangsAPI.Exceptions;
using GangsAPI.Extensions;
using GangsAPI.Permissions;
using GangsAPI.Services;
using GangsAPI.Services.Menu;
using GangsAPI.Services.Player;
using Menu;
using Microsoft.Extensions.DependencyInjection;
using Stats.Stat.Player;

namespace Commands.Menus;

public class MemberMenu(IServiceProvider provider, IGangPlayer member)
  : AbstractMenu<string>(provider.GetRequiredService<IMenuManager>(),
    NativeSenders.Chat) {
  private readonly IRankManager ranks =
    provider.GetRequiredService<IRankManager>();

  private readonly IPlayerManager players =
    provider.GetRequiredService<IPlayerManager>();

  private readonly IPlayerStatManager playerStats =
    provider.GetRequiredService<IPlayerStatManager>();

  private readonly string playtimeId = new PlaytimeStat().StatId;

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

    if (viewerRank.Permissions.HasFlag(Perm.DEMOTE_OTHERS))
      result.Add("Demote");
    if (viewerRank.Permissions.HasFlag(Perm.PROMOTE_OTHERS))
      result.Add("Promote");
    if (viewerRank.Permissions.HasFlag(Perm.KICK_OTHERS)) result.Add("Kick");

    var (success, data) =
      await playerStats.GetForPlayer<PlaytimeData>(player, playtimeId);

    if (!success || data == null) return result;

    result.Add($"Last Played: {data.GetLastPlayed().FormatRelative()}");
    return result;
  }

  override protected Task HandleItemSelection(PlayerWrapper player,
    List<string> items, int selectedIndex) {
    throw new NotImplementedException();
  }

  override protected Task<string> FormatItem(PlayerWrapper player, int index,
    string item) {
    throw new NotImplementedException();
  }
}