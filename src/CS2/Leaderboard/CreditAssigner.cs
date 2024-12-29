using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Timers;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Services;

namespace Leaderboard;

public class CreditAssigner(IEcoManager eco) : IPluginBehavior {
  public void Start(BasePlugin? plugin, bool hotReload) {
    plugin?.AddTimer(30f, refresh, TimerFlags.REPEAT);
  }

  private void refresh() {
    var players = Utilities.GetPlayers()
     .Where(p => !p.IsBot)
     .Select(p => new PlayerWrapper(p))
     .ToList();

    try {
      Task.Run(async () => {
        foreach (var player in players) {
          var balance = await eco.GetBalance(player, true);
          if (player.Player == null) continue;
          await Server.NextFrameAsync(() => {
            if (player.Player.InGameMoneyServices == null) return;
            player.Player.InGameMoneyServices.Account = balance;
            Utilities.SetStateChanged(player.Player, "CCSPlayerController",
              "m_pInGameMoneyServices");
          });
        }
      });
    } catch (Exception e) {
      Console.WriteLine(e);
      throw;
    }
  }
}