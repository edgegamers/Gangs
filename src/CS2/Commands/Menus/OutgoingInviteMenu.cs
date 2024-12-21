using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Gang;
using GangsAPI.Extensions;
using GangsAPI.Menu;
using GangsAPI.Services.Gang;
using Microsoft.Extensions.DependencyInjection;
using Stats.Stat.Gang;
using ICommandManager = GangsAPI.Services.Commands.ICommandManager;

namespace Commands.Menus;

public class OutgoingInviteMenu(IServiceProvider provider, IGang gang,
  ulong invited) : AbstractPagedMenu<string>(provider, NativeSenders.Chat) {
  override protected Task<List<string>> GetItems(PlayerWrapper _) {
    var results = new List<string> { "Cancel" };
    return Task.FromResult(results);
  }

  override protected async Task HandleItemSelection(PlayerWrapper player,
    List<string> items, int selectedIndex) {
    if (selectedIndex != 1) {
      await Provider.GetRequiredService<ICommandManager>()
       .ProcessCommand(player, CommandCallingContext.Chat, "css_gang", "invite",
          "cancel", invited.ToString());
    }
  }

  override protected Task ShowPage(PlayerWrapper player, List<string> items,
    int currentPage, int totalPages) {
    if (currentPage == 0)
      player.PrintToChat(
        $"{ChatColors.DarkRed}!{ChatColors.Red}GANGS {ChatColors.LightRed}Invites {ChatColors.Grey}{gang.Name}");
    return base.ShowPage(player, items, currentPage, totalPages);
  }

  override protected Task<string> FormatItem(PlayerWrapper player, int index,
    string item) {
    return Task.FromResult($"{index}. {item}");
  }
}