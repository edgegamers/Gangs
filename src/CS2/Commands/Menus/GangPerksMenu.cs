using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Perks;
using GangsAPI.Services;
using GangsAPI.Services.Gang;
using GangsAPI.Services.Menu;
using GangsAPI.Services.Player;
using Menu;
using Microsoft.Extensions.DependencyInjection;

namespace Commands.Menus;

public class GangPerksMenu(IServiceProvider provider)
  : AbstractPagedMenu<IPerk>(provider, NativeSenders.Chat) {
  private readonly IGangStatManager gangStats =
    provider.GetRequiredService<IGangStatManager>();

  override protected Task<List<IPerk>> GetItems(PlayerWrapper player) {
    var perks = Provider.GetRequiredService<IPerkManager>().Perks.ToList()!;
    return Task.FromResult(perks.ToList());
  }

  override protected async Task HandleItemSelection(PlayerWrapper player,
    List<IPerk> items, int selectedIndex) {
    var perk = items[selectedIndex];
    player.PrintToChat($"You selected {perk.Name}");
    var gangPlayer = await Provider.GetRequiredService<IPlayerManager>()
     .GetPlayer(player.Steam);
    if (gangPlayer == null) return;

    if (gangPlayer.GangId == null) {
      player.PrintToChat("You are not in a gang");
      return;
    }

    var getForGang = gangStats.GetType().GetMethod("GetForGang");

    if (getForGang == null) {
      player.PrintToChat(Localizer.Get(MSG.GENERIC_ERROR_INFO,
        "Could not find GetForGang"));
      return;
    }

    var getForGangTyped = getForGang.MakeGenericMethod(perk.ValueType);
    dynamic? task = getForGangTyped.Invoke(gangStats,
      [gangPlayer.GangId.Value, perk.StatId]);

    if (task == null) {
      player.PrintToChat(Localizer.Get(MSG.GENERIC_ERROR_INFO,
        $"Could not get stat {perk.StatId}"));
      return;
    }

    // var    result  = await task;
    // bool   success = result.Item1;
    // object value   = result.Item2;
    //
    // if (!success || value is null) return;

    var menu = await perk.GetMenu(gangPlayer);
    if (menu == null) return;
    await Provider.GetRequiredService<IMenuManager>().OpenMenu(player, menu);
  }

  override protected Task<string> FormatItem(PlayerWrapper player, int index,
    IPerk item) {
    return Task.FromResult($"{index} {item.Name}");
  }
}