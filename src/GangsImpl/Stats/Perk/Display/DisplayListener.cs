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

  private async Task applyDisplays(List<IGang> cachedGangs,
    List<IGangPlayer> gangPlayers, List<PlayerWrapper> cachedPlayers) {
    var displayPerk = provider.GetService<IDisplayPerk>();
    if (displayPerk == null) return;

    var displaySetting = provider.GetService<IDisplaySetting>();
    if (displaySetting == null) return;

    Dictionary<int, (bool, bool)> cachedPerks = new();

    foreach (var gangPlayer in gangPlayers) {
      var gang = cachedGangs.FirstOrDefault(g => g.GangId == gangPlayer.GangId);
      if (gang == null || gangPlayer.GangId == null) continue;
      bool chat, scoreboard;
      if (!cachedPerks.TryGetValue(gang.GangId, out var perk)) {
        chat       = await displayPerk.HasChatDisplay(gang);
        scoreboard = await displayPerk.HasScoreboardDisplay(gang);
        cachedPerks.Add(gang.GangId, (chat, scoreboard));
      } else { (chat, scoreboard) = perk; }

      var wrapper =
        cachedPlayers.FirstOrDefault(p => p.Steam == gangPlayer.Steam);
      if (wrapper == null || wrapper.Player == null) continue;
      if (chat && await displaySetting.IsChatEnabled(gangPlayer)) {
        // MAUL
      }

      if (scoreboard && await displaySetting.IsScoreboardEnabled(gangPlayer)) {
        await Server.NextFrameAsync(() => {
          wrapper.Player.Clan = gang.Name;
          Utilities.SetStateChanged(wrapper.Player, "CCSPlayerController",
            "m_szClan");
        });
        wrapper.PrintToChat(
          $"Your gang's name is now displayed on the scoreboard");
      } else {
        wrapper.PrintToChat(
          "Your gang's name is no longer displayed on the scoreboard");
      }
    }
  }
}