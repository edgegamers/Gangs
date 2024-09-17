using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EcoRewards;

public class PeriodicRewarder(IServiceProvider provider) : IPluginBehavior {
  private readonly IEcoManager eco = provider.GetRequiredService<IEcoManager>();

  public void Start(BasePlugin? plugin, bool hotReload) {
    plugin?.AddTimer(60 * 5, () => {
      var players = Utilities.GetPlayers()
       .Where(p => p is { IsBot: false, Team: > CsTeam.Spectator })
       .Select(p => new PlayerWrapper(p));

      foreach (var player in players) {
        if (player.Player == null) continue;
        var reward = player.Player.PawnIsAlive ? 7 : 3;
        Task.Run(
          async () => await eco.Grant(player, reward, reason: "Playtime"));
      }
    });
  }
}