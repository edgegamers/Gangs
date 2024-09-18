using System.Drawing;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Services.Gang;
using GangsAPI.Services.Player;
using Microsoft.Extensions.DependencyInjection;

namespace Stats.Perk.Smoke;

public class SmokeListener(IServiceProvider provider) : IPluginBehavior {
  private readonly Dictionary<ulong, Color> smokeColors = new();

  private readonly IPlayerManager players =
    provider.GetRequiredService<IPlayerManager>();

  private readonly IGangManager gangs =
    provider.GetRequiredService<IGangManager>();

  private readonly IGangStatManager gangStats =
    provider.GetRequiredService<IGangStatManager>();

  public void Start(BasePlugin? plugin, bool hotReload) {
    plugin?.RegisterListener<Listeners.OnEntitySpawned>(OnEntitySpawned);
  }

  private void OnEntitySpawned(CEntityInstance entity) {
    if (entity.DesignerName != "smokegrenade_projectile") return;

    CSmokeGrenadeProjectile proj = new(entity.Handle);
    if (proj.Handle == IntPtr.Zero || !proj.IsValid) return;

    Server.NextFrame(() => {
      var thrower = proj.Thrower.Value?.Controller.Value;
      if (thrower == null || !thrower.IsValid) return;
      if (!smokeColors.TryGetValue(thrower.SteamID, out var color)) return;
      proj.SmokeColor.X = color.R;
      proj.SmokeColor.Y = color.G;
      proj.SmokeColor.Z = color.B;
    });
  }

  [GameEventHandler]
  public HookResult OnRoundStart(EventRoundStart ev, GameEventInfo info) {
    var wrapped = Utilities.GetPlayers()
     .Where(p => !p.IsBot)
     .Select(p => new PlayerWrapper(p))
     .ToList();

    smokeColors.Clear();
    Task.Run(async () => { await fetchSmokeColors(wrapped); });
    return HookResult.Continue;
  }

  private async Task fetchSmokeColors(List<PlayerWrapper> wrappers) {
    Dictionary<int, Color?> cachedGangs = new();
    foreach (var wrapper in wrappers) {
      var player = await players.GetPlayer(wrapper.Steam);
      if (player?.GangId == null) continue;
      if (!cachedGangs.TryGetValue(player.GangId.Value, out var color)) {
        var gang = await gangs.GetGang(player.GangId.Value);
        if (gang == null) continue;
        var (success, data) =
          await gangStats.GetForGang<SmokePerkData>(gang,
            SmokeColorPerk.STAT_ID);
        if (!success || data == null) {
          cachedGangs.Add(player.GangId.Value, null);
          continue;
        }

        color = data.Equipped.GetColor() ?? data.Equipped.PickRandom();
        cachedGangs.Add(player.GangId.Value, color);
      }

      if (color == null) continue;
      smokeColors[wrapper.Steam] = color.Value;
    }
  }
}