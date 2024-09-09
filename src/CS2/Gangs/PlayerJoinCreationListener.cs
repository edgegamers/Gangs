using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using GangsAPI;
using GangsAPI.Services.Player;

namespace GangsImpl;

public class PlayerJoinCreationListener(IPlayerManager mgr) : IPluginBehavior {
  public void Start(BasePlugin? plugin, bool hotReload) {
    if (!hotReload) return;
    foreach (var player in Utilities.GetPlayers().Where(p => !p.IsBot))
      Task.Run(async () => {
        await updatePlayerName(player.SteamID, player.PlayerName);
      });
  }

  [GameEventHandler]
  public HookResult OnJoin(EventPlayerConnectFull ev, GameEventInfo info) {
    var player = ev.Userid;
    if (player == null || !player.IsValid) return HookResult.Continue;

    var steam = player.SteamID;
    var name  = player.PlayerName;

    Task.Run(async () => { await updatePlayerName(steam, name); });
    return HookResult.Continue;
  }

  private async Task updatePlayerName(ulong steam, string name) {
    var gPlayer = await mgr.CreatePlayer(steam, name);

    if (gPlayer.Name == null) {
      gPlayer.Name = name;
      await mgr.UpdatePlayer(gPlayer);
    }
  }
}