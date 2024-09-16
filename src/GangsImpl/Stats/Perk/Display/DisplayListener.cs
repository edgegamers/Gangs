using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Gang;
using GangsAPI.Perks;
using GangsAPI.Services.Gang;
using GangsAPI.Services.Player;
using Microsoft.Extensions.DependencyInjection;

namespace Stats.Perk.Display;

public class DisplayListener(IServiceProvider provider) : IPluginBehavior {
  private readonly IPlayerManager players =
    provider.GetRequiredService<IPlayerManager>();

  private readonly IGangManager gangs =
    provider.GetRequiredService<IGangManager>();

  [GameEventHandler]
  public HookResult OnRoundStart(EventRoundStart ev, GameEventInfo info) {
    var wrapped = Utilities.GetPlayers()
     .Where(p => !p.IsBot)
     .Select(p => new PlayerWrapper(p))
     .ToList();

    Task.Run(async () => {
      var gangPlayers = wrapped
       .Select(async p => await players.GetPlayer(p.Steam))
       .Select(t => t.Result)
       .ToList();

      var cachedGangs = (await gangs.GetGangs()).ToList();

      await applyDisplays(cachedGangs, gangPlayers!, wrapped);
    });
    return HookResult.Continue;
  }

  private async Task applyDisplays(List<IGang> gangs,
    List<IGangPlayer> gangPlayers, List<PlayerWrapper> players) {
    foreach (var gang in gangs) {
      var displayPerk = provider.GetService<IDisplayPerk>();
      if (displayPerk == null) continue;

      var chat       = await displayPerk.HasChatDisplay(gang);
      var scoreboard = await displayPerk.HasScoreboardDisplay(gang);

      foreach (var gangPlayer in gangPlayers) {
        var wrapper = players.FirstOrDefault(p => p.Steam == gangPlayer.Steam);
        if (wrapper == null || wrapper.Player == null) continue;

        var displaySetting = provider.GetService<IDisplaySetting>();
        if (displaySetting == null) break;

        if (chat && await displaySetting.IsChatEnabled(gangPlayer)) {
          // MAUL
        }

        if (scoreboard
          && await displaySetting.IsScoreboardEnabled(gangPlayer)) {
          wrapper.Player.Clan = gang.Name;
          Utilities.SetStateChanged(wrapper.Player, "CCSPlayerController",
            "m_szClan");
        }
      }
    }
  }
}