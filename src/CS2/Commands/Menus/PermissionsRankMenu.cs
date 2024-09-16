using CounterStrikeSharp.API.Modules.Utils;
using GangsAPI.Data;
using GangsAPI.Permissions;
using GangsAPI.Services.Commands;
using Menu;
using Microsoft.Extensions.DependencyInjection;

namespace Commands.Menus;

public class PermissionsRankMenu(IServiceProvider provider,
  List<IGangRank> ranks)
  : AbstractPagedMenu<IGangRank>(provider, NativeSenders.Chat) {
  override protected Task<List<IGangRank>> GetItems(PlayerWrapper player) {
    return Task.FromResult(ranks);
  }

  private readonly ICommandManager commands =
    provider.GetRequiredService<ICommandManager>();

  override protected Task HandleItemSelection(PlayerWrapper player,
    List<IGangRank> items, int selectedIndex) {
    var rank = items[selectedIndex].Rank;

    commands.ProcessCommand(player, "css_gang", "permission", rank.ToString());
    return Task.CompletedTask;
  }

  override protected Task<string> FormatItem(PlayerWrapper player, int index,
    IGangRank item) {
    // Convert rank's permissions to a bitfield string
    var perms = (int)item.Permissions;

    const int maxPerms = (int)Perm.OWNER;
    var       longestRankName = ranks.Max(r => r.Rank.ToString().Length);
    var       maxLength = Convert.ToString(maxPerms, 2).Length;
    var       permString = Convert.ToString(perms, 2).PadLeft(maxLength, '0');
    permString = permString.Replace("0", ChatColors.Red + "-")
     .Replace("1", ChatColors.Green + "x");

    var longestRank = ranks.Max(r => r.Rank.ToString().Length);
    var rankPadded  = item.Rank.ToString().PadLeft(longestRank, '0');

    return Task.FromResult($"{index}. {item.Name} ({rankPadded}) {permString}");
  }
}