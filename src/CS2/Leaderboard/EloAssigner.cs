using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Utils;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Services;

namespace Leaderboard;

public class EloAssigner(ILeaderboard lb) : IPluginBehavior {
  private readonly Dictionary<ulong, int> ranks = new();

  public void Start(BasePlugin? plugin, bool hotReload) {
    plugin?.RegisterListener<Listeners.OnTick>(OnTick);

    if (hotReload) refresh();
  }

  [GameEventHandler]
  public HookResult OnStart(EventRoundStart ev, GameEventInfo info) {
    refresh();
    return HookResult.Continue;
  }

  private void refresh() {
    ranks.Clear();

    var players = Utilities.GetPlayers()
     .Where(player => !player.IsBot && player.Team != CsTeam.Spectator)
     .Select(p => new PlayerWrapper(p))
     .ToList();

    try {
      Task.Run(async () => {
        foreach (var player in players) {
          var elo                               = await lb.GetELO(player.Steam);
          if (elo.HasValue) ranks[player.Steam] = elo.Value;
        }
      });
    } catch (Exception e) {
      Console.WriteLine(e);
      throw;
    }
  }

  private void OnTick() {
    var players = Utilities.GetPlayers()
     .Where(player => !player.IsBot && player.Team != CsTeam.Spectator);

    foreach (var player in players) {
      if (!ranks.TryGetValue(player.SteamID, out var rankInfo)) continue;
      player.CompetitiveRankType = 11;
      player.CompetitiveRanking  = rankInfo;
      player.CompetitiveWins     = 777;
      Utilities.SetStateChanged(player, "CCSPlayerController",
        "m_iCompetitiveRankType");
    }
  }
}