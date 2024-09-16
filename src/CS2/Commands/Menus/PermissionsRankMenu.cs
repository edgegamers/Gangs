using CounterStrikeSharp.API.Modules.Commands;
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
  private readonly ICommandManager commands =
    provider.GetRequiredService<ICommandManager>();

  override protected Task<List<IGangRank>> GetItems(PlayerWrapper player) {
    return Task.FromResult(ranks);
  }

  override protected Task HandleItemSelection(PlayerWrapper player,
    List<IGangRank> items, int selectedIndex) {
    var rank = items[selectedIndex].Rank;

    commands.ProcessCommand(player, CommandCallingContext.Chat, "css_gang",
      "permission", rank.ToString());
    return Task.CompletedTask;
  }

  override protected Task<string> FormatItem(PlayerWrapper player, int index,
    IGangRank item) {
    return Task.FromResult(
      $"{ChatColors.Grey}{index}. {item.Permissions.GetChatBitfield()}{ChatColors.Grey} ({ChatColors.White}{item.Rank}{ChatColors.Grey}): {ChatColors.LightBlue}{item.Name}");
  }
}