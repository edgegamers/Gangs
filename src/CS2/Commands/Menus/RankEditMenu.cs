using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Permissions;
using GangsAPI.Services.Commands;
using GangsAPI.Services.Menu;
using Menu;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace Commands.Menus;

public class RankEditMenu(IServiceProvider provider, IGangRank rank)
  : AbstractMenu<string>(provider.GetRequiredService<IMenuManager>(),
    NativeSenders.Chat) {
  private readonly ICommandManager commands =
    provider.GetRequiredService<ICommandManager>();

  private readonly IStringLocalizer localizer =
    provider.GetRequiredService<IStringLocalizer>();

  override protected Task<List<string>> GetItems(PlayerWrapper player) {
    var result = new List<string> {
      $"{ChatColors.LightRed}Ranks: {ChatColors.LightBlue}{rank.Name} {ChatColors.Orange}[{ChatColors.Yellow}#{rank.Rank}{ChatColors.Orange}]",
      ChatColors.Orange + "Rename",
      ChatColors.Orange + "Delete"
    };

    return Task.FromResult(result);
  }

  override protected Task HandleItemSelection(PlayerWrapper player,
    List<string> items, int selectedIndex) {
    switch (selectedIndex) {
      case 1:
        player.PrintToChat(localizer.Get(MSG.COMMAND_RANK_RENAME_PROMPT,
          rank.Rank));
        break;
      case 2:
        commands.ProcessCommand(player, CommandCallingContext.Chat, "css_gang",
          "rank", "delete", rank.Rank.ToString());
        break;
    }

    return Task.CompletedTask;
  }

  override protected Task<string> FormatItem(PlayerWrapper player, int index,
    string item) {
    if (index == 0) return Task.FromResult(item);
    return Task.FromResult($"{index}. {item}");
  }
}