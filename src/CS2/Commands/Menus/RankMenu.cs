using CounterStrikeSharp.API.Modules.Utils;
using GangsAPI.Data;
using GangsAPI.Data.Gang;
using GangsAPI.Permissions;
using GangsAPI.Services;
using GangsAPI.Services.Menu;
using Menu;
using Microsoft.Extensions.DependencyInjection;

namespace Commands.Menus;

public class RankMenu(IServiceProvider provider, IGang gang)
  : AbstractPagedMenu<IGangRank>(provider, NativeSenders.Chat) {
  private readonly IRankManager ranks =
    provider.GetRequiredService<IRankManager>();

  private int longestRank = -1;

  override protected async Task<List<IGangRank>>
    GetItems(PlayerWrapper player) {
    var gangRanks = (await ranks.GetRanks(gang)).ToList();
    longestRank = gangRanks.Max(r => r.Rank.ToString().Length);
    return gangRanks.ToList();
  }

  override protected Task HandleItemSelection(PlayerWrapper player,
    List<IGangRank> items, int selectedIndex) {
    var rank = items[selectedIndex];
    var menu = new RankEditMenu(provider, rank);
    return Menus.OpenMenu(player, menu);
  }

  override protected Task<string> FormatItem(PlayerWrapper player, int index,
    IGangRank item) {
    var paddedRank = item.Rank.ToString().PadLeft(longestRank, '0');
    var result =
      $"{index}. {ChatColors.Yellow}{paddedRank}{ChatColors.Default} | {getPermColor(item.Permissions)}{item.Name}";
    return Task.FromResult(result);
  }

  private char getPermColor(Perm perm) {
    if (perm.HasFlag(Perm.OWNER)) return ChatColors.DarkRed;
    if (perm.HasFlag(Perm.ADMINISTRATOR)) return ChatColors.Red;
    if (perm.HasFlag(Perm.MANAGE_RANKS)) return ChatColors.DarkBlue;
    if (perm.HasFlag(Perm.DEMOTE_OTHERS)) return ChatColors.Purple;
    if (perm.HasFlag(Perm.MANAGE_INVITES)) return ChatColors.Magenta;
    if (perm.HasFlag(Perm.BANK_WITHDRAW)) return ChatColors.LightBlue;
    if (perm.HasFlag(Perm.BANK_DEPOSIT)) return ChatColors.Blue;
    return ChatColors.Grey;
  }
}