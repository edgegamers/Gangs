using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Menu;
using GangsAPI.Data.Gang;
using GangsAPI.Services.Player;

namespace Commands.Menus;

public class GangMenu : ChatMenu {
  private readonly IGang gang;
  private readonly IPlayerManager playerMgr;

  public GangMenu(IGang gang, IPlayerManager playerMgr) : base(
    $"Gangs - {gang.Name}") {
    this.gang      = gang;
    this.playerMgr = playerMgr;
  }

  public async Task<GangMenu> Load() {
    var members = await playerMgr.GetMembers(gang);
    await Server.NextFrameAsync(()
      => AddMenuOption($"{members.Count()} Members", MembersOption));

    return this;
  }

  public void MembersOption(CCSPlayerController player, ChatMenuOption option) {
    Task.Run(async () => {
      var menu = await new MembersMenu(gang, playerMgr).Load();
      await Server.NextFrameAsync(() => menu.Open(player));
    });
  }
}