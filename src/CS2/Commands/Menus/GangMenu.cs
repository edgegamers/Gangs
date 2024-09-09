using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Menu;
using CounterStrikeSharp.API.Modules.Utils;
using GangsAPI.Data;
using GangsAPI.Data.Gang;
using GangsAPI.Services.Player;
using IMenu = GangsAPI.Services.Menu.IMenu;

namespace Commands.Menus;

public class GangMenu(IGang gang, IPlayerManager playerMgr) : IMenu {
  public async Task Open(PlayerWrapper player) {
    var members = await playerMgr.GetMembers(gang);
    player.PrintToChat($"{ChatColors.Red}Gangs - {gang.Name}");
    player.PrintToChat(
      $"{ChatColors.DarkRed}1 | {ChatColors.Yellow}{members}{ChatColors.LightRed} Members");
  }

  public Task Close(PlayerWrapper player) { return Task.CompletedTask; }

  public Task AcceptInput(PlayerWrapper player, int input) {
    return Task.CompletedTask;
  }
}