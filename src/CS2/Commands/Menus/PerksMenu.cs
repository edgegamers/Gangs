using CounterStrikeSharp.API.Modules.Utils;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Exceptions;
using GangsAPI.Perks;
using GangsAPI.Services;
using GangsAPI.Services.Menu;
using GangsAPI.Services.Player;
using Menu;
using Microsoft.Extensions.DependencyInjection;

namespace Commands.Menus;

public class PerksMenu(IServiceProvider provider)
  : AbstractPagedMenu<IPerk>(provider, NativeSenders.Chat) {
  private readonly IPlayerManager players =
    provider.GetRequiredService<IPlayerManager>();

  override protected Task<List<IPerk>> GetItems(PlayerWrapper player) {
    var perks = Provider.GetRequiredService<IPerkManager>().Perks.ToList();
    return Task.FromResult(perks.ToList());
  }

  override protected async Task HandleItemSelection(PlayerWrapper player,
    List<IPerk> items, int selectedIndex) {
    var perk = items[selectedIndex];
    var gangPlayer = await Provider.GetRequiredService<IPlayerManager>()
     .GetPlayer(player.Steam);
    if (gangPlayer == null) throw new PlayerNotFoundException(player.Steam);

    if (gangPlayer.GangId == null) {
      player.PrintToChat(Localizer.Get(MSG.NOT_IN_GANG));
      return;
    }

    var menu = await perk.GetMenu(gangPlayer);
    if (menu == null) return;
    await Provider.GetRequiredService<IMenuManager>().OpenMenu(player, menu);
  }

  override async protected Task<string> FormatItem(PlayerWrapper player,
    int index, IPerk item) {
    var gangPlayer = await players.GetPlayer(player.Steam)
      ?? throw new PlayerNotFoundException(player.Steam);
    var cost = await item.GetCost(gangPlayer);
    var result = (cost == null) ?
      $"{index}. {ChatColors.Blue}{item.Name}" :
      $"{ChatColors.DarkRed}{index}. {ChatColors.LightBlue}{item.Name} {ChatColors.DarkRed}({ChatColors.Red}{cost}{ChatColors.DarkRed})";
    if (index == 1) result = $" {ChatColors.DarkBlue}Gang Perks\n{result}";
    return result;
  }
}