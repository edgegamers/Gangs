﻿using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Timers;
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
       .Select(p => new PlayerWrapper(p))
       .ToList();

      if (players.Count < RewardsCollection.MIN_PLAYERS) return;

      players = players.Where(p => p.Player != null).ToList();

      foreach (var player in players) {
        var reward = getBaseReward(player);
        
        if (players.Count < 15)
          reward *= 2;
        
        Task.Run(
          async () => await eco.Grant(player, reward, reason: "Playtime"));
      }
    }, TimerFlags.REPEAT);
  }

  private static int getBaseReward(PlayerWrapper player) {
    var alive = player.Player != null && player.Player.PawnIsAlive;
    if (player.HasFlags("@ego/royal")) return alive ? 21 : 14;
    if (player.HasFlags("@ego/platinum")) return alive ? 14 : 10;
    if (player.HasFlags("@ego/dsgold")) return alive ? 10 : 7;
    if (player.HasFlags("@ego/dssilver")) return alive ? 8 : 5;
    return alive ? 5 : 3;
  }
}