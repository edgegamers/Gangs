using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Menu;
using GangsAPI.Data.Gang;
using GangsAPI.Services.Player;

namespace Commands.Menus;

public class MembersMenu(IGang gang, IPlayerManager playerMgr)
  : ChatMenu($"Gang Members - {gang.Name}") {
  public async Task<MembersMenu> Load() {
    var members = await playerMgr.GetMembers(gang);
    await Server.NextFrameAsync(() => {
      foreach (var member in members) {
        AddMenuOption(member.Name ?? member.Steam.ToString(),
          (player, option) => { }, true);
      }
    });

    return this;
  }
}